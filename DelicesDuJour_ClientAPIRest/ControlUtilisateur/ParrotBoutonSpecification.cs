using ReaLTaiizor.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelicesDuJour_ClientAPIRest.ControlUtilisateur
{
    internal class ParrotBoutonSpecification : ParrotButton, IButtonControl
    {

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new Color BackColor { get; set; }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new Color ForeColor { get; set; }
        public DialogResult DialogResult { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ParrotBoutonSpecification()
        {
            base.SetStyle(ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
            base.Size = new Size(200, 50);
            CurrentBackColor = backColor;
            CurrentForeColor = foreColor;
            BackColor = Color.Transparent;
            Cursor = Cursors.Hand;
        }


        /// <summary>
        ///  Notifies the <see cref="Button"/>
        ///  whether it is the default button so that it can adjust its appearance
        ///  accordingly.
        /// </summary>
        public virtual void NotifyDefault(bool value)
        {
         
        }

        /// <summary>
        ///  Generates a <see cref="Control.Click"/> event for a
        ///  button.
        /// </summary>
        public void PerformClick()
        {
         
        }

        private Color CurrentBackColor;

        private Color CurrentForeColor;

        private Color foreColor = Color.DodgerBlue;

        private Color backColor = Color.FromArgb(255, 255, 255);
    }
}
