namespace IconTest {
	partial class IconTest {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
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
			this.iconDisplay = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.iconDisplay)).BeginInit();
			this.SuspendLayout();
			// 
			// iconDisplay
			// 
			this.iconDisplay.Location = new System.Drawing.Point(13, 13);
			this.iconDisplay.Name = "iconDisplay";
			this.iconDisplay.Size = new System.Drawing.Size(64, 64);
			this.iconDisplay.TabIndex = 0;
			this.iconDisplay.TabStop = false;
			// 
			// IconTest
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(192, 93);
			this.Controls.Add(this.iconDisplay);
			this.Name = "IconTest";
			this.Text = "IconTest";
			((System.ComponentModel.ISupportInitialize)(this.iconDisplay)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox iconDisplay;
	}
}