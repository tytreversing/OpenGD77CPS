using System.Drawing;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace DMR;

public class TreeForm : ToolWindow
{
	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
            this.SuspendLayout();
            // 
            // TreeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(331, 349);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)((((WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "TreeForm";
            this.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockLeft;
            this.TabText = "TreeView";
            this.Text = "TreeView";
            this.ResumeLayout(false);

	}

	public TreeForm()
	{
		InitializeComponent();
		Scale(Settings.smethod_6());
	}
}
