using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace DualViewRoiComparator.Forms
{
    /// <summary>
    /// Prompts for the geometry of a headerless raw .yuv file (I420 4:2:0). Built entirely in
    /// code (no designer / resx) so it can be dropped in without extra project wiring.
    /// </summary>
    public sealed class YuvSpecDialog : Form
    {
        private readonly TextBox _txtWidth;
        private readonly TextBox _txtHeight;
        private readonly TextBox _txtFps;

        public int FrameWidth { get; private set; }
        public int FrameHeight { get; private set; }
        public double Fps { get; private set; }

        public YuvSpecDialog()
        {
            Text = "YUV 정보 입력 (I420 4:2:0)";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(320, 190);

            Label info = new Label
            {
                Text = "raw .yuv 파일은 헤더가 없어 해상도/FPS가 필요합니다.\n형식은 8비트 I420(YUV420p)로 가정합니다.",
                Location = new Point(12, 10),
                Size = new Size(296, 40)
            };

            Label lblW = new Label { Text = "가로(Width)", Location = new Point(12, 58), AutoSize = true };
            _txtWidth = new TextBox { Text = "1920", Location = new Point(120, 55), Size = new Size(180, 23) };

            Label lblH = new Label { Text = "세로(Height)", Location = new Point(12, 88), AutoSize = true };
            _txtHeight = new TextBox { Text = "1080", Location = new Point(120, 85), Size = new Size(180, 23) };

            Label lblF = new Label { Text = "FPS", Location = new Point(12, 118), AutoSize = true };
            _txtFps = new TextBox { Text = "30", Location = new Point(120, 115), Size = new Size(180, 23) };

            Button ok = new Button { Text = "확인", Location = new Point(120, 150), Size = new Size(85, 30) };
            ok.Click += OnOk;
            Button cancel = new Button { Text = "취소", Location = new Point(215, 150), Size = new Size(85, 30) };
            cancel.Click += delegate { DialogResult = DialogResult.Cancel; Close(); };

            Controls.Add(info);
            Controls.Add(lblW); Controls.Add(_txtWidth);
            Controls.Add(lblH); Controls.Add(_txtHeight);
            Controls.Add(lblF); Controls.Add(_txtFps);
            Controls.Add(ok); Controls.Add(cancel);
            AcceptButton = ok;
            CancelButton = cancel;
        }

        private void OnOk(object sender, EventArgs e)
        {
            int w, h;
            double fps;
            if (!int.TryParse(_txtWidth.Text.Trim(), out w) || w <= 0 ||
                !int.TryParse(_txtHeight.Text.Trim(), out h) || h <= 0)
            {
                MessageBox.Show(this, "가로/세로는 양의 정수여야 합니다.", "입력 오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
                return;
            }
            if (!double.TryParse(_txtFps.Text.Trim(), NumberStyles.Float,
                    CultureInfo.InvariantCulture, out fps) || fps <= 0)
                fps = 30.0;

            FrameWidth = w;
            FrameHeight = h;
            Fps = fps;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
