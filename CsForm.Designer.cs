
namespace AcadCsObjectsTransform
{
    partial class CsForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.startButton = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.initialCsComboBox = new System.Windows.Forms.ComboBox();
            this.targetCsComboBox = new System.Windows.Forms.ComboBox();
            this.initialCsLabel = new System.Windows.Forms.Label();
            this.targetCsLabel = new System.Windows.Forms.Label();
            this.initialXOffsetLabel = new System.Windows.Forms.Label();
            this.initialXOffsetTextBox = new System.Windows.Forms.TextBox();
            this.initialYOffsetTextBox = new System.Windows.Forms.TextBox();
            this.initialYOffsetLabel = new System.Windows.Forms.Label();
            this.targetXOfssetLabel = new System.Windows.Forms.Label();
            this.targetXOffsetTextBox = new System.Windows.Forms.TextBox();
            this.targetYOffsetTextBox = new System.Windows.Forms.TextBox();
            this.targetYOffsetLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(725, 249);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(94, 29);
            this.startButton.TabIndex = 0;
            this.startButton.Text = "Запуск";
            this.startButton.UseVisualStyleBackColor = true;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 249);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(707, 29);
            this.progressBar.Step = 1;
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar.TabIndex = 1;
            // 
            // initialCsComboBox
            // 
            this.initialCsComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.initialCsComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.initialCsComboBox.FormattingEnabled = true;
            this.initialCsComboBox.Location = new System.Drawing.Point(117, 26);
            this.initialCsComboBox.Name = "initialCsComboBox";
            this.initialCsComboBox.Size = new System.Drawing.Size(702, 28);
            this.initialCsComboBox.TabIndex = 2;
            // 
            // targetCsComboBox
            // 
            this.targetCsComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.targetCsComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.targetCsComboBox.FormattingEnabled = true;
            this.targetCsComboBox.Location = new System.Drawing.Point(117, 137);
            this.targetCsComboBox.Name = "targetCsComboBox";
            this.targetCsComboBox.Size = new System.Drawing.Size(702, 28);
            this.targetCsComboBox.TabIndex = 3;
            // 
            // initialCsLabel
            // 
            this.initialCsLabel.AutoSize = true;
            this.initialCsLabel.Location = new System.Drawing.Point(12, 29);
            this.initialCsLabel.Name = "initialCsLabel";
            this.initialCsLabel.Size = new System.Drawing.Size(98, 20);
            this.initialCsLabel.TabIndex = 4;
            this.initialCsLabel.Text = "Исходная СК";
            // 
            // targetCsLabel
            // 
            this.targetCsLabel.AutoSize = true;
            this.targetCsLabel.Location = new System.Drawing.Point(13, 140);
            this.targetCsLabel.Name = "targetCsLabel";
            this.targetCsLabel.Size = new System.Drawing.Size(90, 20);
            this.targetCsLabel.TabIndex = 5;
            this.targetCsLabel.Text = "Целевая СК";
            // 
            // initialXOffsetLabel
            // 
            this.initialXOffsetLabel.AutoSize = true;
            this.initialXOffsetLabel.Location = new System.Drawing.Point(117, 72);
            this.initialXOffsetLabel.Name = "initialXOffsetLabel";
            this.initialXOffsetLabel.Size = new System.Drawing.Size(190, 20);
            this.initialXOffsetLabel.TabIndex = 6;
            this.initialXOffsetLabel.Text = "Смещение по X(AutoCAD)";
            // 
            // initialXOffsetTextBox
            // 
            this.initialXOffsetTextBox.Location = new System.Drawing.Point(313, 69);
            this.initialXOffsetTextBox.Name = "initialXOffsetTextBox";
            this.initialXOffsetTextBox.Size = new System.Drawing.Size(120, 27);
            this.initialXOffsetTextBox.TabIndex = 7;
            this.initialXOffsetTextBox.Text = "0.0";
            // 
            // initialYOffsetTextBox
            // 
            this.initialYOffsetTextBox.Location = new System.Drawing.Point(699, 69);
            this.initialYOffsetTextBox.Name = "initialYOffsetTextBox";
            this.initialYOffsetTextBox.Size = new System.Drawing.Size(120, 27);
            this.initialYOffsetTextBox.TabIndex = 8;
            this.initialYOffsetTextBox.Text = "0.0";
            // 
            // initialYOffsetLabel
            // 
            this.initialYOffsetLabel.AutoSize = true;
            this.initialYOffsetLabel.Location = new System.Drawing.Point(499, 72);
            this.initialYOffsetLabel.Name = "initialYOffsetLabel";
            this.initialYOffsetLabel.Size = new System.Drawing.Size(189, 20);
            this.initialYOffsetLabel.TabIndex = 9;
            this.initialYOffsetLabel.Text = "Смещение по Y(AutoCAD)";
            // 
            // targetXOfssetLabel
            // 
            this.targetXOfssetLabel.AutoSize = true;
            this.targetXOfssetLabel.Location = new System.Drawing.Point(117, 183);
            this.targetXOfssetLabel.Name = "targetXOfssetLabel";
            this.targetXOfssetLabel.Size = new System.Drawing.Size(190, 20);
            this.targetXOfssetLabel.TabIndex = 10;
            this.targetXOfssetLabel.Text = "Смещение по X(AutoCAD)";
            // 
            // targetXOffsetTextBox
            // 
            this.targetXOffsetTextBox.Location = new System.Drawing.Point(313, 180);
            this.targetXOffsetTextBox.Name = "targetXOffsetTextBox";
            this.targetXOffsetTextBox.Size = new System.Drawing.Size(120, 27);
            this.targetXOffsetTextBox.TabIndex = 11;
            this.targetXOffsetTextBox.Text = "0.0";
            // 
            // targetYOffsetTextBox
            // 
            this.targetYOffsetTextBox.Location = new System.Drawing.Point(699, 180);
            this.targetYOffsetTextBox.Name = "targetYOffsetTextBox";
            this.targetYOffsetTextBox.Size = new System.Drawing.Size(120, 27);
            this.targetYOffsetTextBox.TabIndex = 12;
            this.targetYOffsetTextBox.Text = "0.0";
            // 
            // targetYOffsetLabel
            // 
            this.targetYOffsetLabel.AutoSize = true;
            this.targetYOffsetLabel.Location = new System.Drawing.Point(499, 183);
            this.targetYOffsetLabel.Name = "targetYOffsetLabel";
            this.targetYOffsetLabel.Size = new System.Drawing.Size(189, 20);
            this.targetYOffsetLabel.TabIndex = 13;
            this.targetYOffsetLabel.Text = "Смещение по Y(AutoCAD)";
            // 
            // CsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(832, 303);
            this.Controls.Add(this.targetYOffsetLabel);
            this.Controls.Add(this.targetYOffsetTextBox);
            this.Controls.Add(this.targetXOffsetTextBox);
            this.Controls.Add(this.targetXOfssetLabel);
            this.Controls.Add(this.initialYOffsetLabel);
            this.Controls.Add(this.initialYOffsetTextBox);
            this.Controls.Add(this.initialXOffsetTextBox);
            this.Controls.Add(this.initialXOffsetLabel);
            this.Controls.Add(this.targetCsLabel);
            this.Controls.Add(this.initialCsLabel);
            this.Controls.Add(this.targetCsComboBox);
            this.Controls.Add(this.initialCsComboBox);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.startButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "CsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Преобразование координат";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.ComboBox initialCsComboBox;
        private System.Windows.Forms.ComboBox targetCsComboBox;
        private System.Windows.Forms.Label initialCsLabel;
        private System.Windows.Forms.Label targetCsLabel;
        private System.Windows.Forms.Label initialXOffsetLabel;
        private System.Windows.Forms.TextBox initialXOffsetTextBox;
        private System.Windows.Forms.TextBox initialYOffsetTextBox;
        private System.Windows.Forms.Label initialYOffsetLabel;
        private System.Windows.Forms.Label targetXOfssetLabel;
        private System.Windows.Forms.TextBox targetXOffsetTextBox;
        private System.Windows.Forms.TextBox targetYOffsetTextBox;
        private System.Windows.Forms.Label targetYOffsetLabel;
    }
}

