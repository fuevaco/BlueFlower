namespace BlueFlower
{
    partial class BlueAPP
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.Log = new System.Windows.Forms.TextBox();
            this.FindBLE = new System.Windows.Forms.Button();
            this.DeviceList = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // Log
            // 
            this.Log.Location = new System.Drawing.Point(8, 232);
            this.Log.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Log.Multiline = true;
            this.Log.Name = "Log";
            this.Log.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.Log.Size = new System.Drawing.Size(516, 194);
            this.Log.TabIndex = 1;
            // 
            // FindBLE
            // 
            this.FindBLE.Location = new System.Drawing.Point(10, 19);
            this.FindBLE.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.FindBLE.Name = "FindBLE";
            this.FindBLE.Size = new System.Drawing.Size(132, 34);
            this.FindBLE.TabIndex = 3;
            this.FindBLE.Text = "尋找附近藍芽";
            this.FindBLE.UseVisualStyleBackColor = true;
            this.FindBLE.Click += new System.EventHandler(this.FindBLE_Click);
            // 
            // DeviceList
            // 
            this.DeviceList.FormattingEnabled = true;
            this.DeviceList.ItemHeight = 12;
            this.DeviceList.Location = new System.Drawing.Point(155, 19);
            this.DeviceList.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.DeviceList.Name = "DeviceList";
            this.DeviceList.Size = new System.Drawing.Size(369, 196);
            this.DeviceList.TabIndex = 4;
            this.DeviceList.SelectedIndexChanged += new System.EventHandler(this.DeviceList_SelectedIndexChanged);
            // 
            // BlueAPP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(535, 435);
            this.Controls.Add(this.DeviceList);
            this.Controls.Add(this.FindBLE);
            this.Controls.Add(this.Log);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "BlueAPP";
            this.Text = "花花草草藍牙測試";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox Log;
        private System.Windows.Forms.Button FindBLE;
        private System.Windows.Forms.ListBox DeviceList;
    }
}

