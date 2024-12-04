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
		base.SuspendLayout();
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 12f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(284, 262);
		base.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom;
		this.Font = new System.Drawing.Font("Arial", 10f, System.Drawing.FontStyle.Regular);
		base.Name = "TreeForm";
		base.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
		base.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockLeft;
		base.TabText = "TreeView";
		this.Text = "TreeView";
		base.ResumeLayout(false);
	}

	public TreeForm()
	{
		InitializeComponent();
		Scale(Settings.smethod_6());
	}
}
