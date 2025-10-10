// (c) gfoidl, all rights reserved

namespace WinFormsAnimation;

partial class Form1
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
        components = new System.ComponentModel.Container();
        panel1 = new BufferedPanel();
        animationTimer = new System.Windows.Forms.Timer(components);
        showTrajectoryCheckBox = new CheckBox();
        saveImagesCheckBox = new CheckBox();
        showCrossHairsCheckBox = new CheckBox();
        statusStrip1 = new StatusStrip();
        iterationStripStatusLabel = new ToolStripStatusLabel();
        statusStrip1.SuspendLayout();
        this.SuspendLayout();
        // 
        // panel1
        // 
        panel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        panel1.BackColor = SystemColors.Control;
        panel1.Location = new Point(12, 41);
        panel1.Name = "panel1";
        panel1.Size = new Size(558, 290);
        panel1.TabIndex = 0;
        panel1.Paint += this.panel1_Paint;
        // 
        // animationTimer
        // 
        animationTimer.Interval = 50;
        animationTimer.Tick += this.animationTimer_Tick;
        // 
        // showTrajectoryCheckBox
        // 
        showTrajectoryCheckBox.AutoSize = true;
        showTrajectoryCheckBox.Checked = true;
        showTrajectoryCheckBox.CheckState = CheckState.Checked;
        showTrajectoryCheckBox.Location = new Point(12, 12);
        showTrajectoryCheckBox.Name = "showTrajectoryCheckBox";
        showTrajectoryCheckBox.Size = new Size(122, 23);
        showTrajectoryCheckBox.TabIndex = 1;
        showTrajectoryCheckBox.Text = "show trajectory";
        showTrajectoryCheckBox.UseVisualStyleBackColor = true;
        // 
        // saveImagesCheckBox
        // 
        saveImagesCheckBox.AutoSize = true;
        saveImagesCheckBox.Checked = true;
        saveImagesCheckBox.CheckState = CheckState.Checked;
        saveImagesCheckBox.Location = new Point(468, 12);
        saveImagesCheckBox.Name = "saveImagesCheckBox";
        saveImagesCheckBox.Size = new Size(102, 23);
        saveImagesCheckBox.TabIndex = 2;
        saveImagesCheckBox.Text = "save images";
        saveImagesCheckBox.UseVisualStyleBackColor = true;
        // 
        // showCrossHairsCheckBox
        // 
        showCrossHairsCheckBox.AutoSize = true;
        showCrossHairsCheckBox.Checked = true;
        showCrossHairsCheckBox.CheckState = CheckState.Checked;
        showCrossHairsCheckBox.Location = new Point(140, 12);
        showCrossHairsCheckBox.Name = "showCrossHairsCheckBox";
        showCrossHairsCheckBox.Size = new Size(124, 23);
        showCrossHairsCheckBox.TabIndex = 3;
        showCrossHairsCheckBox.Text = "show crosshairs";
        showCrossHairsCheckBox.UseVisualStyleBackColor = true;
        // 
        // statusStrip1
        // 
        statusStrip1.ImageScalingSize = new Size(18, 18);
        statusStrip1.Items.AddRange(new ToolStripItem[] { iterationStripStatusLabel });
        statusStrip1.Location = new Point(0, 334);
        statusStrip1.Name = "statusStrip1";
        statusStrip1.Size = new Size(582, 22);
        statusStrip1.TabIndex = 4;
        statusStrip1.Text = "statusStrip1";
        // 
        // iterationStripStatusLabel
        // 
        iterationStripStatusLabel.Name = "iterationStripStatusLabel";
        iterationStripStatusLabel.Size = new Size(0, 17);
        // 
        // Form1
        // 
        this.AutoScaleDimensions = new SizeF(8F, 19F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.ClientSize = new Size(582, 356);
        this.Controls.Add(statusStrip1);
        this.Controls.Add(showCrossHairsCheckBox);
        this.Controls.Add(saveImagesCheckBox);
        this.Controls.Add(showTrajectoryCheckBox);
        this.Controls.Add(panel1);
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;
        this.Name = "Form1";
        this.Text = "Cairo Animation Demo";
        statusStrip1.ResumeLayout(false);
        statusStrip1.PerformLayout();
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    #endregion

    private BufferedPanel panel1;
    private System.Windows.Forms.Timer animationTimer;
    private CheckBox showTrajectoryCheckBox;
    private CheckBox saveImagesCheckBox;
    private CheckBox showCrossHairsCheckBox;
    private StatusStrip statusStrip1;
    private ToolStripStatusLabel iterationStripStatusLabel;
}
