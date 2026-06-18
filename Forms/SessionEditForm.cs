using System;
using System.Windows.Forms;

namespace DualViewRoiComparator.Forms
{
    /// <summary>
    /// Modal dialog used both for creating a new session (entering a name + memo) and for
    /// editing an existing session's metadata. Performs a basic non-empty name check before
    /// allowing OK; uniqueness is enforced by the SessionManager when saving.
    /// </summary>
    public partial class SessionEditForm : Form
    {
        public string SessionName
        {
            get { return txtName.Text.Trim(); }
            set { txtName.Text = value; }
        }

        public string Memo
        {
            get { return txtMemo.Text; }
            set { txtMemo.Text = value; }
        }

        public SessionEditForm()
        {
            InitializeComponent();
        }

        public SessionEditForm(string title, string name, string memo) : this()
        {
            Text = title;
            txtName.Text = name ?? string.Empty;
            txtMemo.Text = memo ?? string.Empty;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show(this, "세션 이름을 입력하세요.", "입력 오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
                txtName.Focus();
                return;
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
