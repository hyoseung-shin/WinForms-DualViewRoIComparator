using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using DualViewRoiComparator.Heatmap;

namespace DualViewRoiComparator.Persistence
{
    /// <summary>
    /// JSON file-based persistence layer implementing full CRUD over analysis sessions.
    /// Individual sessions live in <c>/Sessions/{SessionId}.json</c>; a lightweight
    /// <c>/Sessions/index.json</c> holds the summaries used to populate the session list.
    /// All public methods validate input and translate low-level IO/JSON faults into
    /// user-facing <see cref="SessionPersistenceException"/>s.
    /// </summary>
    public sealed class SessionManager
    {
        private readonly string _sessionsDir;
        private readonly string _indexPath;
        private readonly JsonSerializerSettings _settings;

        public SessionManager(string baseDirectory)
        {
            if (string.IsNullOrEmpty(baseDirectory))
                baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            _sessionsDir = Path.Combine(baseDirectory, "Sessions");
            _indexPath = Path.Combine(_sessionsDir, "index.json");
            _settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Include
            };

            EnsureStorage();
        }

        public string SessionsDirectory { get { return _sessionsDir; } }

        // ----------------------------------------------------------------- CREATE
        public SessionData Create(SessionData data)
        {
            if (data == null) throw new ArgumentNullException("data");

            ValidateName(data.Name, null);

            data.SessionId = GenerateSessionId();
            data.CreatedAt = DateTime.Now;
            data.UpdatedAt = data.CreatedAt;

            SaveHeatmapImage(data); // also writes /Sessions/{id}.png and sets HeatmapImagePath
            WriteSessionFile(data);

            var index = LoadIndexSafe();
            index.Add(SessionSummary.FromData(data));
            WriteIndex(index);

            return data;
        }

        // ------------------------------------------------------------------- READ
        public List<SessionSummary> ReadAll()
        {
            return LoadIndexSafe()
                .OrderByDescending(s => s.UpdatedAt)
                .ToList();
        }

        public SessionData ReadById(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new SessionPersistenceException("세션 식별자가 비어 있습니다.");

            string path = SessionPath(id);
            if (!File.Exists(path))
                throw new SessionPersistenceException("세션 파일을 찾을 수 없습니다: " + id);

            try
            {
                string json = File.ReadAllText(path);
                var data = JsonConvert.DeserializeObject<SessionData>(json, _settings);
                if (data == null)
                    throw new SessionPersistenceException("세션 데이터를 읽을 수 없습니다 (빈 파일).");
                return data;
            }
            catch (JsonException ex)
            {
                throw new SessionPersistenceException("세션 파일이 손상되어 파싱할 수 없습니다: " + id, ex);
            }
            catch (IOException ex)
            {
                throw new SessionPersistenceException("세션 파일을 읽는 중 오류가 발생했습니다: " + ex.Message, ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new SessionPersistenceException("세션 파일에 접근 권한이 없습니다: " + ex.Message, ex);
            }
        }

        // ----------------------------------------------------------------- UPDATE
        public SessionData Update(string id, SessionData updated)
        {
            if (string.IsNullOrEmpty(id))
                throw new SessionPersistenceException("세션 식별자가 비어 있습니다.");
            if (updated == null) throw new ArgumentNullException("updated");

            string path = SessionPath(id);
            if (!File.Exists(path))
                throw new SessionPersistenceException("수정할 세션을 찾을 수 없습니다: " + id);

            ValidateName(updated.Name, id);

            // Preserve identity & creation timestamp.
            SessionData existing = ReadById(id);
            updated.SessionId = id;
            updated.CreatedAt = existing.CreatedAt;
            updated.UpdatedAt = DateTime.Now;

            SaveHeatmapImage(updated);
            WriteSessionFile(updated);

            var index = LoadIndexSafe();
            index.RemoveAll(s => s.SessionId == id);
            index.Add(SessionSummary.FromData(updated));
            WriteIndex(index);

            return updated;
        }

        // ----------------------------------------------------------------- DELETE
        public void Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new SessionPersistenceException("세션 식별자가 비어 있습니다.");

            try
            {
                string path = SessionPath(id);
                if (File.Exists(path))
                    File.Delete(path);
                string img = ImagePath(id);
                if (File.Exists(img))
                    File.Delete(img);
            }
            catch (IOException ex)
            {
                throw new SessionPersistenceException("세션 파일 삭제 중 오류가 발생했습니다: " + ex.Message, ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new SessionPersistenceException("세션 파일 삭제 권한이 없습니다: " + ex.Message, ex);
            }

            var index = LoadIndexSafe();
            index.RemoveAll(s => s.SessionId == id);
            WriteIndex(index);
        }

        // ---------------------------------------------------------------- helpers
        private void EnsureStorage()
        {
            try
            {
                if (!Directory.Exists(_sessionsDir))
                    Directory.CreateDirectory(_sessionsDir);
                if (!File.Exists(_indexPath))
                    WriteIndex(new List<SessionSummary>());
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new SessionPersistenceException(
                    "세션 폴더를 생성할 권한이 없습니다. 프로그램을 다른 위치에서 실행하거나 권한을 확인하세요.", ex);
            }
            catch (IOException ex)
            {
                throw new SessionPersistenceException("세션 저장소 초기화 중 오류가 발생했습니다: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Loads the index. Individual corrupt entries are tolerated: if the index file itself
        /// is unreadable it is rebuilt from the session files present on disk.
        /// </summary>
        private List<SessionSummary> LoadIndexSafe()
        {
            try
            {
                if (!File.Exists(_indexPath))
                    return RebuildIndexFromDisk();

                string json = File.ReadAllText(_indexPath);
                var list = JsonConvert.DeserializeObject<List<SessionSummary>>(json, _settings);
                if (list == null)
                    return RebuildIndexFromDisk();

                // Drop entries whose backing file disappeared.
                return list.Where(s => s != null && !string.IsNullOrEmpty(s.SessionId)
                                       && File.Exists(SessionPath(s.SessionId))).ToList();
            }
            catch (JsonException)
            {
                return RebuildIndexFromDisk();
            }
            catch (IOException)
            {
                return new List<SessionSummary>();
            }
        }

        private List<SessionSummary> RebuildIndexFromDisk()
        {
            var summaries = new List<SessionSummary>();
            try
            {
                if (!Directory.Exists(_sessionsDir))
                    return summaries;

                foreach (string file in Directory.GetFiles(_sessionsDir, "*.json"))
                {
                    if (string.Equals(Path.GetFileName(file), "index.json", StringComparison.OrdinalIgnoreCase))
                        continue;
                    try
                    {
                        string json = File.ReadAllText(file);
                        var data = JsonConvert.DeserializeObject<SessionData>(json, _settings);
                        if (data != null && !string.IsNullOrEmpty(data.SessionId))
                            summaries.Add(SessionSummary.FromData(data));
                    }
                    catch (JsonException)
                    {
                        // Skip individual corrupt files but keep scanning the rest.
                    }
                }
                WriteIndex(summaries);
            }
            catch (IOException)
            {
                // Best effort; return whatever was gathered.
            }
            return summaries;
        }

        private void ValidateName(string name, string ignoreId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new SessionPersistenceException("세션 이름은 공백일 수 없습니다.");

            var index = LoadIndexSafe();
            bool duplicate = index.Any(s =>
                (ignoreId == null || s.SessionId != ignoreId) &&
                string.Equals(s.Name, name.Trim(), StringComparison.OrdinalIgnoreCase));

            if (duplicate)
                throw new SessionPersistenceException("같은 이름의 세션이 이미 존재합니다: " + name.Trim());
        }

        private void WriteSessionFile(SessionData data)
        {
            try
            {
                string json = JsonConvert.SerializeObject(data, _settings);
                File.WriteAllText(SessionPath(data.SessionId), json);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new SessionPersistenceException(
                    "세션을 저장할 권한이 없습니다. 폴더 권한을 확인하세요: " + _sessionsDir, ex);
            }
            catch (IOException ex)
            {
                throw new SessionPersistenceException("세션 저장 중 디스크 오류가 발생했습니다: " + ex.Message, ex);
            }
        }

        private void WriteIndex(List<SessionSummary> index)
        {
            try
            {
                string json = JsonConvert.SerializeObject(index, _settings);
                File.WriteAllText(_indexPath, json);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new SessionPersistenceException(
                    "세션 목록(index.json)을 저장할 권한이 없습니다.", ex);
            }
            catch (IOException ex)
            {
                throw new SessionPersistenceException("세션 목록 저장 중 오류가 발생했습니다: " + ex.Message, ex);
            }
        }

        private string SessionPath(string id)
        {
            return Path.Combine(_sessionsDir, id + ".json");
        }

        private string ImagePath(string id)
        {
            return Path.Combine(_sessionsDir, id + ".png");
        }

        /// <summary>
        /// Renders the accumulated heatmap grid to a PNG next to the JSON file and records its
        /// absolute path on the session. Image export failures are non-fatal (JSON still saved).
        /// </summary>
        private void SaveHeatmapImage(SessionData data)
        {
            if (data.HeatmapGrid == null) { data.HeatmapImagePath = null; return; }

            string imgPath = ImagePath(data.SessionId);
            try
            {
                using (Bitmap bmp = ColorMapHelper.Render(data.HeatmapGrid, 640, 360))
                {
                    bmp.Save(imgPath, ImageFormat.Png);
                }
                data.HeatmapImagePath = imgPath;
            }
            catch (Exception)
            {
                data.HeatmapImagePath = null; // keep JSON save working even if image export fails
            }
        }

        private string GenerateSessionId()
        {
            // Timestamp-based id with a short random suffix to avoid same-second collisions.
            string baseId = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            string candidate = baseId;
            int suffix = 1;
            while (File.Exists(SessionPath(candidate)))
            {
                candidate = baseId + "-" + suffix.ToString();
                suffix++;
            }
            return candidate;
        }
    }
}
