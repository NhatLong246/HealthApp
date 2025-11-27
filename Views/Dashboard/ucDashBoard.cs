using System;
using System.Linq;
using System.Windows.Forms;
using HealthApp.Views.Nutrition;

namespace HealthApp.Views.Dashboard
{
    public partial class ucDashBoard : UserControl
    {
        private frmDashBoard _parentForm;

        public ucDashBoard()
        {
            InitializeComponent();
        }

        public ucDashBoard(frmDashBoard parentForm) : this()
        {
            _parentForm = parentForm;
        }

        private void ucDashBoard_Load(object sender, EventArgs e)
        {
            AttachMealPlanNavigation(pnlKeHoachAnUong);
            AttachMealPlanNavigation(label9);
            AttachMealPlanNavigation(guna2PictureBox1);
        }

        private void AttachMealPlanNavigation(Control control)
        {
            if (control == null) return;
            control.Cursor = Cursors.Hand;
            control.Click -= NavigateToMealPlan;
            control.Click += NavigateToMealPlan;
        }

        private void NavigateToMealPlan(object sender, EventArgs e)
        {
            var parentForm = _parentForm
                ?? this.FindForm() as frmDashBoard
                ?? Application.OpenForms.OfType<frmDashBoard>().FirstOrDefault();

            if (parentForm != null)
            {
                parentForm.LoadUserControl(new ucCheDoAnUongDeXuat());
            }
            else
            {
                MessageBox.Show("Không thể mở trang kế hoạch ăn uống.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
