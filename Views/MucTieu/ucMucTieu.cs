using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using HealthApp.Views.Dashboard;

namespace HealthApp.Views.MucTieu
{
    public partial class ucMucTieu : UserControl
    {
        public ucMucTieu()
        {
            InitializeComponent();
        }

        private void ucMucTieu_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Event handler cho button Trở Về - điều hướng về trang chủ (Dashboard)
        /// </summary>
        private void btnTroVe_Click(object sender, EventArgs e)
        {
            try
            {
                // Tìm frmDashBoard
                frmDashBoard parentForm = null;

                // Cách 1: Tìm qua FindForm()
                Form form = this.FindForm();
                if (form is frmDashBoard)
                {
                    parentForm = form as frmDashBoard;
                }
                // Cách 2: Tìm qua Application.OpenForms
                else
                {
                    foreach (Form openForm in Application.OpenForms)
                    {
                        if (openForm is frmDashBoard)
                        {
                            parentForm = openForm as frmDashBoard;
                            break;
                        }
                    }
                }

                if (parentForm != null)
                {
                    // Tạo và load ucDashBoard (trang chủ)
                    ucDashBoard ucDashBoard = new ucDashBoard(parentForm);
                    parentForm.LoadUserControl(ucDashBoard);
                }
                else
                {
                    MessageBox.Show("Không thể tìm thấy form chính để điều hướng.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi điều hướng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
