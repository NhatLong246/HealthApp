using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace HealthApp.Views.Nutrition
{
    // FlowLayoutPanel tùy chỉnh để ẩn scrollbar nhưng vẫn có thể cuộn
    // Cách đơn giản: chỉ ẩn visual scrollbar, để base class xử lý tất cả logic cuộn
    public class FlowLayoutPanelNoScrollbar : FlowLayoutPanel
    {
        [DllImport("user32.dll")]
        private static extern int ShowScrollBar(IntPtr hWnd, int wBar, bool bShow);

        private const int SB_HORZ = 0;
        private const int SB_VERT = 1;

        public FlowLayoutPanelNoScrollbar()
        {
            // Không cần timer, chỉ ẩn khi cần thiết
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            // Ẩn scrollbar ngay khi handle được tạo
            if (IsHandleCreated)
            {
                HideScrollBars();
            }
        }

        protected override void WndProc(ref Message m)
        {
            // Để base class xử lý TẤT CẢ messages trước (bao gồm cuộn)
            base.WndProc(ref m);
            
            // Sau khi base class xử lý xong, ẩn scrollbar nếu cần
            // Chỉ ẩn sau các message liên quan đến scroll/paint
            const int WM_HSCROLL = 0x114;
            const int WM_VSCROLL = 0x115;
            const int WM_PAINT = 0x000F;
            const int WM_MOUSEWHEEL = 0x020A;
            
            if (m.Msg == WM_HSCROLL || m.Msg == WM_VSCROLL || m.Msg == WM_MOUSEWHEEL || m.Msg == WM_PAINT)
            {
                HideScrollBars();
            }
        }

        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            // Ẩn scrollbar sau khi resize
            if (IsHandleCreated)
            {
                HideScrollBars();
            }
        }

        private void HideScrollBars()
        {
            if (!IsHandleCreated || !AutoScroll)
                return;

            try
            {
                // Chỉ ẩn visual scrollbar bằng Windows API
                // Không thay đổi bất kỳ logic nào của AutoScroll
                ShowScrollBar(Handle, SB_HORZ, false);
                ShowScrollBar(Handle, SB_VERT, false);
            }
            catch { }
        }
    }
}

