// <copyright file="RenderPanel.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

namespace CrusaderKingsStoryGen
{
    using System.Windows.Forms;

    public partial class RenderPanel : PictureBox
    {
        public RenderPanel()
        {
            this.InitializeComponent();
            this.DoubleBuffered = true;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
        }
    }
}
