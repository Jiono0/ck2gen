// <copyright file="AlertForm.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

namespace CrusaderKingsStoryGen.Forms
{
    using System;
    using System.Windows.Forms;

    public partial class AlertForm : Form
    {
        #region PROPERTIES

        public string Message
        {
            set { this.labelMessage.Text = value; }
        }

        public int ProgressValue
        {
            set { this.progressBar1.Value = value; }
        }

        #endregion

        #region METHODS

        public AlertForm()
        {
            this.InitializeComponent();
        }

        #endregion

        #region EVENTS

        public event EventHandler<EventArgs> Canceled;

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            // Create a copy of the event to work with
            EventHandler<EventArgs> ea = this.Canceled;
            /* If there are no subscribers, eh will be null so we need to check
             * to avoid a NullReferenceException. */
            if (ea != null)
            {
                ea(this, e);
            }
        }

        #endregion

    }
}
