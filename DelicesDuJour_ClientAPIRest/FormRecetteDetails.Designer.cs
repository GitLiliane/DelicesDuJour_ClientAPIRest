namespace DelicesDuJour_ClientAPIRest
{
    partial class FormRecetteDetails
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRecetteDetails));
            tableLayoutPanel1 = new TableLayoutPanel();
            tableLayoutPanel2 = new TableLayoutPanel();
            pictureBox1 = new PictureBox();
            tableLayoutPanel3 = new TableLayoutPanel();
            tableLayoutPanel4 = new TableLayoutPanel();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            txtdifficulteAfficherRecette = new TextBox();
            dtTempsPreparationAfficherRecette = new DateTimePicker();
            dtTempsCuissonAfficherRecette = new DateTimePicker();
            groupBox1 = new GroupBox();
            rchListIngredientAfficherRecette = new RichTextBox();
            groupBox2 = new GroupBox();
            txtListeEtapesAfficherRecette = new TextBox();
            txtNomAfficherRecette = new TextBox();
            BSRecetteById = new BindingSource(components);
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            tableLayoutPanel3.SuspendLayout();
            tableLayoutPanel4.SuspendLayout();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)BSRecetteById).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 0, 1);
            tableLayoutPanel1.Controls.Add(groupBox2, 0, 2);
            tableLayoutPanel1.Controls.Add(txtNomAfficherRecette, 0, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 8F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 46F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 46F));
            tableLayoutPanel1.Size = new Size(853, 1005);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            tableLayoutPanel2.Controls.Add(pictureBox1, 0, 0);
            tableLayoutPanel2.Controls.Add(tableLayoutPanel3, 1, 0);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(3, 83);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Size = new Size(847, 456);
            tableLayoutPanel2.TabIndex = 0;
            // 
            // pictureBox1
            // 
            pictureBox1.Dock = DockStyle.Fill;
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.InitialImage = null;
            pictureBox1.Location = new Point(3, 3);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(502, 450);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 1;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.Controls.Add(tableLayoutPanel4, 0, 0);
            tableLayoutPanel3.Controls.Add(groupBox1, 0, 1);
            tableLayoutPanel3.Dock = DockStyle.Fill;
            tableLayoutPanel3.Location = new Point(511, 3);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 2;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 75F));
            tableLayoutPanel3.Size = new Size(333, 450);
            tableLayoutPanel3.TabIndex = 1;
            // 
            // tableLayoutPanel4
            // 
            tableLayoutPanel4.ColumnCount = 2;
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            tableLayoutPanel4.Controls.Add(label1, 0, 0);
            tableLayoutPanel4.Controls.Add(label2, 0, 1);
            tableLayoutPanel4.Controls.Add(label3, 0, 2);
            tableLayoutPanel4.Controls.Add(txtdifficulteAfficherRecette, 1, 2);
            tableLayoutPanel4.Controls.Add(dtTempsPreparationAfficherRecette, 1, 0);
            tableLayoutPanel4.Controls.Add(dtTempsCuissonAfficherRecette, 1, 1);
            tableLayoutPanel4.Dock = DockStyle.Fill;
            tableLayoutPanel4.Location = new Point(3, 3);
            tableLayoutPanel4.Name = "tableLayoutPanel4";
            tableLayoutPanel4.RowCount = 3;
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel4.Size = new Size(327, 106);
            tableLayoutPanel4.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Fill;
            label1.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
            label1.Location = new Point(3, 0);
            label1.Name = "label1";
            label1.Size = new Size(222, 35);
            label1.TabIndex = 0;
            label1.Text = "Temps de préparation :";
            label1.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Dock = DockStyle.Fill;
            label2.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
            label2.Location = new Point(3, 35);
            label2.Name = "label2";
            label2.Size = new Size(222, 35);
            label2.TabIndex = 1;
            label2.Text = "Temps de cuisson :";
            label2.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Dock = DockStyle.Fill;
            label3.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
            label3.Location = new Point(3, 70);
            label3.Name = "label3";
            label3.Size = new Size(222, 36);
            label3.TabIndex = 2;
            label3.Text = "Difficulté : ";
            label3.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtdifficulteAfficherRecette
            // 
            txtdifficulteAfficherRecette.Dock = DockStyle.Fill;
            txtdifficulteAfficherRecette.Location = new Point(231, 73);
            txtdifficulteAfficherRecette.Name = "txtdifficulteAfficherRecette";
            txtdifficulteAfficherRecette.Size = new Size(93, 27);
            txtdifficulteAfficherRecette.TabIndex = 5;
            // 
            // dtTempsPreparationAfficherRecette
            // 
            dtTempsPreparationAfficherRecette.Format = DateTimePickerFormat.Time;
            dtTempsPreparationAfficherRecette.Location = new Point(231, 3);
            dtTempsPreparationAfficherRecette.Name = "dtTempsPreparationAfficherRecette";
            dtTempsPreparationAfficherRecette.ShowUpDown = true;
            dtTempsPreparationAfficherRecette.Size = new Size(93, 27);
            dtTempsPreparationAfficherRecette.TabIndex = 6;
            // 
            // dtTempsCuissonAfficherRecette
            // 
            dtTempsCuissonAfficherRecette.Format = DateTimePickerFormat.Time;
            dtTempsCuissonAfficherRecette.Location = new Point(231, 38);
            dtTempsCuissonAfficherRecette.Name = "dtTempsCuissonAfficherRecette";
            dtTempsCuissonAfficherRecette.ShowUpDown = true;
            dtTempsCuissonAfficherRecette.Size = new Size(93, 27);
            dtTempsCuissonAfficherRecette.TabIndex = 7;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(rchListIngredientAfficherRecette);
            groupBox1.Dock = DockStyle.Fill;
            groupBox1.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            groupBox1.Location = new Point(3, 115);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(327, 332);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "Ingrédients";
            // 
            // rchListIngredientAfficherRecette
            // 
            rchListIngredientAfficherRecette.Dock = DockStyle.Fill;
            rchListIngredientAfficherRecette.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            rchListIngredientAfficherRecette.Location = new Point(3, 30);
            rchListIngredientAfficherRecette.Name = "rchListIngredientAfficherRecette";
            rchListIngredientAfficherRecette.Size = new Size(321, 299);
            rchListIngredientAfficherRecette.TabIndex = 0;
            rchListIngredientAfficherRecette.Text = "";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(txtListeEtapesAfficherRecette);
            groupBox2.Dock = DockStyle.Fill;
            groupBox2.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            groupBox2.Location = new Point(3, 545);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(847, 457);
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            groupBox2.Text = "Étapes de préparation";
            // 
            // txtListeEtapesAfficherRecette
            // 
            txtListeEtapesAfficherRecette.Dock = DockStyle.Fill;
            txtListeEtapesAfficherRecette.Location = new Point(3, 30);
            txtListeEtapesAfficherRecette.Multiline = true;
            txtListeEtapesAfficherRecette.Name = "txtListeEtapesAfficherRecette";
            txtListeEtapesAfficherRecette.Size = new Size(841, 424);
            txtListeEtapesAfficherRecette.TabIndex = 0;
            // 
            // txtNomAfficherRecette
            // 
            txtNomAfficherRecette.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtNomAfficherRecette.Font = new Font("Calibri", 24F, FontStyle.Bold, GraphicsUnit.Point, 0);
            txtNomAfficherRecette.ForeColor = Color.RoyalBlue;
            txtNomAfficherRecette.Location = new Point(3, 20);
            txtNomAfficherRecette.Margin = new Padding(3, 20, 3, 3);
            txtNomAfficherRecette.Multiline = true;
            txtNomAfficherRecette.Name = "txtNomAfficherRecette";
            txtNomAfficherRecette.Size = new Size(847, 57);
            txtNomAfficherRecette.TabIndex = 2;
            txtNomAfficherRecette.TextAlign = HorizontalAlignment.Center;
            // 
            // FormRecetteDetails
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(853, 1005);
            Controls.Add(tableLayoutPanel1);
            Name = "FormRecetteDetails";
            Text = "Détails recette";
            Load += FormRecetteDetails_Load;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel4.ResumeLayout(false);
            tableLayoutPanel4.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)BSRecetteById).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel2;
        private PictureBox pictureBox1;
        private TableLayoutPanel tableLayoutPanel3;
        private TableLayoutPanel tableLayoutPanel4;
        private Label label1;
        private Label label2;
        private Label label3;
        private TextBox txtdifficulteAfficherRecette;
        private GroupBox groupBox1;
        private TextBox textBox1;
        private GroupBox groupBox2;
        private TextBox txtNomAfficherRecette;
        private BindingSource BSRecetteById;
        private DateTimePicker dtTempsPreparationAfficherRecette;
        private DateTimePicker dtTempsCuissonAfficherRecette;
        private TextBox txtListeEtapesAfficherRecette;
        private RichTextBox rchListIngredientAfficherRecette;
    }
}