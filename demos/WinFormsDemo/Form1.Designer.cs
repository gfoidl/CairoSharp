// (c) gfoidl, all rights reserved

namespace WinFormsDemo;

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
        menuStrip1 = new MenuStrip();
        selectASampleToolStripMenuItem = new ToolStripMenuItem();
        arcToolStripMenuItem = new ToolStripMenuItem();
        arcNegativeToolStripMenuItem = new ToolStripMenuItem();
        clipToolStripMenuItem = new ToolStripMenuItem();
        clipImageToolStripMenuItem = new ToolStripMenuItem();
        curvedRectangleToolStripMenuItem = new ToolStripMenuItem();
        roundedRectangleToolStripMenuItem = new ToolStripMenuItem();
        curveToToolStripMenuItem = new ToolStripMenuItem();
        dashToolStripMenuItem = new ToolStripMenuItem();
        fillAndStroke2ToolStripMenuItem = new ToolStripMenuItem();
        fillStyleToolStripMenuItem = new ToolStripMenuItem();
        gradientToolStripMenuItem = new ToolStripMenuItem();
        imageToolStripMenuItem = new ToolStripMenuItem();
        imagePatternToolStripMenuItem = new ToolStripMenuItem();
        multiSegmentCapsToolStripMenuItem = new ToolStripMenuItem();
        setLineCapToolStripMenuItem = new ToolStripMenuItem();
        setLineJoinToolStripMenuItem = new ToolStripMenuItem();
        textToolStripMenuItem = new ToolStripMenuItem();
        textAlignCenterToolStripMenuItem = new ToolStripMenuItem();
        textExtentsToolStripMenuItem = new ToolStripMenuItem();
        glyphsToolStripMenuItem1 = new ToolStripMenuItem();
        glyphsToolStripMenuItem = new ToolStripMenuItem();
        hitTestToolStripMenuItem = new ToolStripMenuItem();
        saveAsPNGToolStripMenuItem = new ToolStripMenuItem();
        drawPanel = new Panel();
        menuStrip1.SuspendLayout();
        this.SuspendLayout();
        // 
        // menuStrip1
        // 
        menuStrip1.ImageScalingSize = new Size(18, 18);
        menuStrip1.Items.AddRange(new ToolStripItem[] { selectASampleToolStripMenuItem, saveAsPNGToolStripMenuItem });
        menuStrip1.Location = new Point(0, 0);
        menuStrip1.Name = "menuStrip1";
        menuStrip1.Size = new Size(382, 27);
        menuStrip1.TabIndex = 0;
        menuStrip1.Text = "menuStrip1";
        // 
        // selectASampleToolStripMenuItem
        // 
        selectASampleToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { arcToolStripMenuItem, arcNegativeToolStripMenuItem, clipToolStripMenuItem, clipImageToolStripMenuItem, curvedRectangleToolStripMenuItem, roundedRectangleToolStripMenuItem, curveToToolStripMenuItem, dashToolStripMenuItem, fillAndStroke2ToolStripMenuItem, fillStyleToolStripMenuItem, gradientToolStripMenuItem, imageToolStripMenuItem, imagePatternToolStripMenuItem, multiSegmentCapsToolStripMenuItem, setLineCapToolStripMenuItem, setLineJoinToolStripMenuItem, textToolStripMenuItem, textAlignCenterToolStripMenuItem, textExtentsToolStripMenuItem, glyphsToolStripMenuItem1, glyphsToolStripMenuItem, hitTestToolStripMenuItem });
        selectASampleToolStripMenuItem.Name = "selectASampleToolStripMenuItem";
        selectASampleToolStripMenuItem.Size = new Size(116, 23);
        selectASampleToolStripMenuItem.Text = "Select a sample";
        // 
        // arcToolStripMenuItem
        // 
        arcToolStripMenuItem.Name = "arcToolStripMenuItem";
        arcToolStripMenuItem.Size = new Size(205, 24);
        arcToolStripMenuItem.Text = "arc";
        arcToolStripMenuItem.Click += this.arcToolStripMenuItem_Click;
        // 
        // arcNegativeToolStripMenuItem
        // 
        arcNegativeToolStripMenuItem.Name = "arcNegativeToolStripMenuItem";
        arcNegativeToolStripMenuItem.Size = new Size(205, 24);
        arcNegativeToolStripMenuItem.Text = "arc negative";
        arcNegativeToolStripMenuItem.Click += this.arcNegativeToolStripMenuItem_Click;
        // 
        // clipToolStripMenuItem
        // 
        clipToolStripMenuItem.Name = "clipToolStripMenuItem";
        clipToolStripMenuItem.Size = new Size(205, 24);
        clipToolStripMenuItem.Text = "clip";
        clipToolStripMenuItem.Click += this.clipToolStripMenuItem_Click;
        // 
        // clipImageToolStripMenuItem
        // 
        clipImageToolStripMenuItem.Name = "clipImageToolStripMenuItem";
        clipImageToolStripMenuItem.Size = new Size(205, 24);
        clipImageToolStripMenuItem.Text = "clip image";
        clipImageToolStripMenuItem.Click += this.clipImageToolStripMenuItem_Click;
        // 
        // curvedRectangleToolStripMenuItem
        // 
        curvedRectangleToolStripMenuItem.Name = "curvedRectangleToolStripMenuItem";
        curvedRectangleToolStripMenuItem.Size = new Size(205, 24);
        curvedRectangleToolStripMenuItem.Text = "curved rectangle";
        curvedRectangleToolStripMenuItem.Click += this.curvedRectangleToolStripMenuItem_Click;
        // 
        // roundedRectangleToolStripMenuItem
        // 
        roundedRectangleToolStripMenuItem.Name = "roundedRectangleToolStripMenuItem";
        roundedRectangleToolStripMenuItem.Size = new Size(205, 24);
        roundedRectangleToolStripMenuItem.Text = "rounded rectangle";
        roundedRectangleToolStripMenuItem.Click += this.roundedRectangleToolStripMenuItem_Click;
        // 
        // curveToToolStripMenuItem
        // 
        curveToToolStripMenuItem.Name = "curveToToolStripMenuItem";
        curveToToolStripMenuItem.Size = new Size(205, 24);
        curveToToolStripMenuItem.Text = "curve to";
        curveToToolStripMenuItem.Click += this.curveToToolStripMenuItem_Click;
        // 
        // dashToolStripMenuItem
        // 
        dashToolStripMenuItem.Name = "dashToolStripMenuItem";
        dashToolStripMenuItem.Size = new Size(205, 24);
        dashToolStripMenuItem.Text = "dash";
        dashToolStripMenuItem.Click += this.dashToolStripMenuItem_Click;
        // 
        // fillAndStroke2ToolStripMenuItem
        // 
        fillAndStroke2ToolStripMenuItem.Name = "fillAndStroke2ToolStripMenuItem";
        fillAndStroke2ToolStripMenuItem.Size = new Size(205, 24);
        fillAndStroke2ToolStripMenuItem.Text = "fill and stroke2";
        fillAndStroke2ToolStripMenuItem.Click += this.fillAndStroke2ToolStripMenuItem_Click;
        // 
        // fillStyleToolStripMenuItem
        // 
        fillStyleToolStripMenuItem.Name = "fillStyleToolStripMenuItem";
        fillStyleToolStripMenuItem.Size = new Size(205, 24);
        fillStyleToolStripMenuItem.Text = "fill style";
        fillStyleToolStripMenuItem.Click += this.fillStyleToolStripMenuItem_Click;
        // 
        // gradientToolStripMenuItem
        // 
        gradientToolStripMenuItem.Name = "gradientToolStripMenuItem";
        gradientToolStripMenuItem.Size = new Size(205, 24);
        gradientToolStripMenuItem.Text = "gradient";
        gradientToolStripMenuItem.Click += this.gradientToolStripMenuItem_Click;
        // 
        // imageToolStripMenuItem
        // 
        imageToolStripMenuItem.Name = "imageToolStripMenuItem";
        imageToolStripMenuItem.Size = new Size(205, 24);
        imageToolStripMenuItem.Text = "image";
        imageToolStripMenuItem.Click += this.imageToolStripMenuItem_Click;
        // 
        // imagePatternToolStripMenuItem
        // 
        imagePatternToolStripMenuItem.Name = "imagePatternToolStripMenuItem";
        imagePatternToolStripMenuItem.Size = new Size(205, 24);
        imagePatternToolStripMenuItem.Text = "image pattern";
        imagePatternToolStripMenuItem.Click += this.imagePatternToolStripMenuItem_Click;
        // 
        // multiSegmentCapsToolStripMenuItem
        // 
        multiSegmentCapsToolStripMenuItem.Name = "multiSegmentCapsToolStripMenuItem";
        multiSegmentCapsToolStripMenuItem.Size = new Size(205, 24);
        multiSegmentCapsToolStripMenuItem.Text = "multi segment caps";
        multiSegmentCapsToolStripMenuItem.Click += this.multiSegmentCapsToolStripMenuItem_Click;
        // 
        // setLineCapToolStripMenuItem
        // 
        setLineCapToolStripMenuItem.Name = "setLineCapToolStripMenuItem";
        setLineCapToolStripMenuItem.Size = new Size(205, 24);
        setLineCapToolStripMenuItem.Text = "set line cap";
        setLineCapToolStripMenuItem.Click += this.setLineCapToolStripMenuItem_Click;
        // 
        // setLineJoinToolStripMenuItem
        // 
        setLineJoinToolStripMenuItem.Name = "setLineJoinToolStripMenuItem";
        setLineJoinToolStripMenuItem.Size = new Size(205, 24);
        setLineJoinToolStripMenuItem.Text = "set line join";
        setLineJoinToolStripMenuItem.Click += this.setLineJoinToolStripMenuItem_Click;
        // 
        // textToolStripMenuItem
        // 
        textToolStripMenuItem.Name = "textToolStripMenuItem";
        textToolStripMenuItem.Size = new Size(205, 24);
        textToolStripMenuItem.Text = "text";
        textToolStripMenuItem.Click += this.textToolStripMenuItem_Click;
        // 
        // textAlignCenterToolStripMenuItem
        // 
        textAlignCenterToolStripMenuItem.Name = "textAlignCenterToolStripMenuItem";
        textAlignCenterToolStripMenuItem.Size = new Size(205, 24);
        textAlignCenterToolStripMenuItem.Text = "text align center";
        textAlignCenterToolStripMenuItem.Click += this.textAlignCenterToolStripMenuItem_Click;
        // 
        // textExtentsToolStripMenuItem
        // 
        textExtentsToolStripMenuItem.Name = "textExtentsToolStripMenuItem";
        textExtentsToolStripMenuItem.Size = new Size(205, 24);
        textExtentsToolStripMenuItem.Text = "text extents";
        textExtentsToolStripMenuItem.Click += this.textExtentsToolStripMenuItem_Click;
        // 
        // glyphsToolStripMenuItem1
        // 
        glyphsToolStripMenuItem1.Name = "glyphsToolStripMenuItem1";
        glyphsToolStripMenuItem1.Size = new Size(205, 24);
        glyphsToolStripMenuItem1.Text = "glyphs";
        glyphsToolStripMenuItem1.Click += this.glyphsToolStripMenuItem1_Click;
        // 
        // glyphsToolStripMenuItem
        // 
        glyphsToolStripMenuItem.Name = "glyphsToolStripMenuItem";
        glyphsToolStripMenuItem.Size = new Size(205, 24);
        glyphsToolStripMenuItem.Text = "glyph extents";
        glyphsToolStripMenuItem.Click += this.glyphsToolStripMenuItem_Click;
        // 
        // hitTestToolStripMenuItem
        // 
        hitTestToolStripMenuItem.Name = "hitTestToolStripMenuItem";
        hitTestToolStripMenuItem.Size = new Size(205, 24);
        hitTestToolStripMenuItem.Text = "hit test";
        hitTestToolStripMenuItem.Click += this.hitTestToolStripMenuItem_Click;
        // 
        // saveAsPNGToolStripMenuItem
        // 
        saveAsPNGToolStripMenuItem.Name = "saveAsPNGToolStripMenuItem";
        saveAsPNGToolStripMenuItem.Size = new Size(100, 23);
        saveAsPNGToolStripMenuItem.Text = "Save as PNG";
        saveAsPNGToolStripMenuItem.Click += this.saveAsPNGToolStripMenuItem_Click;
        // 
        // drawPanel
        // 
        drawPanel.BackColor = SystemColors.Control;
        drawPanel.Location = new Point(12, 30);
        drawPanel.Margin = new Padding(10);
        drawPanel.Name = "drawPanel";
        drawPanel.Size = new Size(256, 256);
        drawPanel.TabIndex = 1;
        drawPanel.Paint += this.drawPanel_Paint;
        drawPanel.MouseClick += this.drawPanel_MouseClick;
        // 
        // Form1
        // 
        this.AutoScaleDimensions = new SizeF(8F, 19F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.AutoSize = true;
        this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        this.ClientSize = new Size(382, 356);
        this.Controls.Add(drawPanel);
        this.Controls.Add(menuStrip1);
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MainMenuStrip = menuStrip1;
        this.MaximizeBox = false;
        this.Name = "Form1";
        this.Text = "CairoSamples";
        menuStrip1.ResumeLayout(false);
        menuStrip1.PerformLayout();
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    #endregion

    private MenuStrip menuStrip1;
    private ToolStripMenuItem selectASampleToolStripMenuItem;
    private ToolStripMenuItem saveAsPNGToolStripMenuItem;
    private ToolStripMenuItem arcToolStripMenuItem;
    private ToolStripMenuItem arcNegativeToolStripMenuItem;
    private ToolStripMenuItem clipToolStripMenuItem;
    private ToolStripMenuItem clipImageToolStripMenuItem;
    private ToolStripMenuItem curvedRectangleToolStripMenuItem;
    private ToolStripMenuItem curveToToolStripMenuItem;
    private ToolStripMenuItem dashToolStripMenuItem;
    private ToolStripMenuItem fillAndStroke2ToolStripMenuItem;
    private ToolStripMenuItem fillStyleToolStripMenuItem;
    private ToolStripMenuItem gradientToolStripMenuItem;
    private ToolStripMenuItem imageToolStripMenuItem;
    private ToolStripMenuItem imagePatternToolStripMenuItem;
    private ToolStripMenuItem multiSegmentCapsToolStripMenuItem;
    private ToolStripMenuItem roundedRectangleToolStripMenuItem;
    private ToolStripMenuItem setLineCapToolStripMenuItem;
    private ToolStripMenuItem setLineJoinToolStripMenuItem;
    private ToolStripMenuItem textToolStripMenuItem;
    private ToolStripMenuItem textAlignCenterToolStripMenuItem;
    private ToolStripMenuItem textExtentsToolStripMenuItem;
    private ToolStripMenuItem glyphsToolStripMenuItem;
    private ToolStripMenuItem glyphsToolStripMenuItem1;
    private ToolStripMenuItem hitTestToolStripMenuItem;
    private Panel drawPanel;
}
