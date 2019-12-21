namespace PskOnline.X20.Playground.GUI
{
  partial class Form1
  {
    /// <summary>
    /// Обязательная переменная конструктора.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Освободить все используемые ресурсы.
    /// </summary>
    /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Код, автоматически созданный конструктором форм Windows

    /// <summary>
    /// Требуемый метод для поддержки конструктора — не изменяйте 
    /// содержимое этого метода с помощью редактора кода.
    /// </summary>
    private void InitializeComponent()
    {
            this.components = new System.ComponentModel.Container();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.plotView1 = new OxyPlot.WindowsForms.PlotView();
            this.useRamp = new System.Windows.Forms.CheckBox();
            this.findPlotsButton = new System.Windows.Forms.Button();
            this.labelSerialNumber = new System.Windows.Forms.Label();
            this.labelRevision = new System.Windows.Forms.Label();
            this.labelBuildDate = new System.Windows.Forms.Label();
            this.checkBoxDoRecord = new System.Windows.Forms.CheckBox();
            this.textBoxFileNamePrefix = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // plotView1
            // 
            this.plotView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.plotView1.Location = new System.Drawing.Point(0, 0);
            this.plotView1.Margin = new System.Windows.Forms.Padding(2);
            this.plotView1.Name = "plotView1";
            this.plotView1.PanCursor = System.Windows.Forms.Cursors.Hand;
            this.plotView1.Size = new System.Drawing.Size(961, 507);
            this.plotView1.TabIndex = 0;
            this.plotView1.Text = "plotView1";
            this.plotView1.ZoomHorizontalCursor = System.Windows.Forms.Cursors.SizeWE;
            this.plotView1.ZoomRectangleCursor = System.Windows.Forms.Cursors.SizeNWSE;
            this.plotView1.ZoomVerticalCursor = System.Windows.Forms.Cursors.SizeNS;
            // 
            // useRamp
            // 
            this.useRamp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.useRamp.AutoSize = true;
            this.useRamp.Location = new System.Drawing.Point(11, 539);
            this.useRamp.Name = "useRamp";
            this.useRamp.Size = new System.Drawing.Size(54, 17);
            this.useRamp.TabIndex = 1;
            this.useRamp.Text = "Ramp";
            this.useRamp.UseVisualStyleBackColor = true;
            this.useRamp.CheckedChanged += new System.EventHandler(this.useRamp_CheckedChanged);
            // 
            // findPlotsButton
            // 
            this.findPlotsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.findPlotsButton.Location = new System.Drawing.Point(77, 535);
            this.findPlotsButton.Name = "findPlotsButton";
            this.findPlotsButton.Size = new System.Drawing.Size(75, 23);
            this.findPlotsButton.TabIndex = 2;
            this.findPlotsButton.Text = "Find";
            this.findPlotsButton.UseVisualStyleBackColor = true;
            this.findPlotsButton.Click += new System.EventHandler(this.findPlotsButton_Click);
            // 
            // labelSerialNumber
            // 
            this.labelSerialNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelSerialNumber.AutoSize = true;
            this.labelSerialNumber.Location = new System.Drawing.Point(188, 540);
            this.labelSerialNumber.Name = "labelSerialNumber";
            this.labelSerialNumber.Size = new System.Drawing.Size(35, 13);
            this.labelSerialNumber.TabIndex = 3;
            this.labelSerialNumber.Text = "label1";
            // 
            // labelRevision
            // 
            this.labelRevision.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelRevision.AutoSize = true;
            this.labelRevision.Location = new System.Drawing.Point(311, 540);
            this.labelRevision.Name = "labelRevision";
            this.labelRevision.Size = new System.Drawing.Size(35, 13);
            this.labelRevision.TabIndex = 4;
            this.labelRevision.Text = "label1";
            // 
            // labelBuildDate
            // 
            this.labelBuildDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelBuildDate.AutoSize = true;
            this.labelBuildDate.Location = new System.Drawing.Point(468, 540);
            this.labelBuildDate.Name = "labelBuildDate";
            this.labelBuildDate.Size = new System.Drawing.Size(35, 13);
            this.labelBuildDate.TabIndex = 4;
            this.labelBuildDate.Text = "label1";
            // 
            // checkBoxDoRecord
            // 
            this.checkBoxDoRecord.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxDoRecord.AutoSize = true;
            this.checkBoxDoRecord.Location = new System.Drawing.Point(655, 540);
            this.checkBoxDoRecord.Name = "checkBoxDoRecord";
            this.checkBoxDoRecord.Size = new System.Drawing.Size(163, 17);
            this.checkBoxDoRecord.TabIndex = 5;
            this.checkBoxDoRecord.Text = "Record data to file with prefix";
            this.checkBoxDoRecord.UseVisualStyleBackColor = true;
            this.checkBoxDoRecord.CheckedChanged += new System.EventHandler(this.checkBoxDoRecord_CheckedChanged);
            // 
            // textBoxFileNamePrefix
            // 
            this.textBoxFileNamePrefix.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxFileNamePrefix.Location = new System.Drawing.Point(832, 538);
            this.textBoxFileNamePrefix.Name = "textBoxFileNamePrefix";
            this.textBoxFileNamePrefix.Size = new System.Drawing.Size(98, 20);
            this.textBoxFileNamePrefix.TabIndex = 6;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(963, 568);
            this.Controls.Add(this.textBoxFileNamePrefix);
            this.Controls.Add(this.checkBoxDoRecord);
            this.Controls.Add(this.labelBuildDate);
            this.Controls.Add(this.labelRevision);
            this.Controls.Add(this.labelSerialNumber);
            this.Controls.Add(this.findPlotsButton);
            this.Controls.Add(this.useRamp);
            this.Controls.Add(this.plotView1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion
    private System.Windows.Forms.Timer timer1;
    private OxyPlot.WindowsForms.PlotView plotView1;
    private System.Windows.Forms.CheckBox useRamp;
    private System.Windows.Forms.Button findPlotsButton;
    private System.Windows.Forms.Label labelSerialNumber;
    private System.Windows.Forms.Label labelRevision;
    private System.Windows.Forms.Label labelBuildDate;
    private System.Windows.Forms.CheckBox checkBoxDoRecord;
    private System.Windows.Forms.TextBox textBoxFileNamePrefix;
  }
}

