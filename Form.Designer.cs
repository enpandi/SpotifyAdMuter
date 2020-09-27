﻿/*
 * App.Designer.cs
 * GUI for the SpotifyAdMuter application
 * Windows Forms, .NET Framework 4.0
 * generated by Visual Studio 2019
 */

namespace SpotifyAdMuter {
	partial class Form {
		private System.ComponentModel.IContainer components = null;

		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form));
			this.Loop = new System.Windows.Forms.Timer(this.components);
			this.muteToggleButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// Loop
			// 
			this.Loop.Interval = 500;
			this.Loop.Tick += new System.EventHandler(this.Loop_Tick);
			// 
			// muteToggleButton
			// 
			this.muteToggleButton.AutoSize = true;
			this.muteToggleButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("muteToggleButton.BackgroundImage")));
			this.muteToggleButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.muteToggleButton.Dock = System.Windows.Forms.DockStyle.Fill;
			this.muteToggleButton.Location = new System.Drawing.Point(0, 0);
			this.muteToggleButton.Name = "muteToggleButton";
			this.muteToggleButton.Size = new System.Drawing.Size(192, 192);
			this.muteToggleButton.TabIndex = 0;
			this.muteToggleButton.Click += new System.EventHandler(this.muteToggleButton_Click);
			// 
			// Form
			// 
			this.ClientSize = new System.Drawing.Size(224, 224);
			this.Controls.Add(this.muteToggleButton);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "Form";
			this.Text = "SAM";
			this.Load += new System.EventHandler(this.App_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Timer Loop;
		private System.Windows.Forms.Button muteToggleButton;
	}
}