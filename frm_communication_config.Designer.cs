namespace 端口侦听控制系统
{
    partial class frm_communication_config
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
            this.btn_communication_config_cancel = new System.Windows.Forms.Button();
            this.btn_communication_config_save = new System.Windows.Forms.Button();
            this.grp_communication_config = new System.Windows.Forms.GroupBox();
            this.cmb_communication_config_parity = new System.Windows.Forms.ComboBox();
            this.cmb_communication_config_stopbit = new System.Windows.Forms.ComboBox();
            this.lbl_communication_config_parity = new System.Windows.Forms.Label();
            this.lbl_communication_config_stopbit = new System.Windows.Forms.Label();
            this.txtbox_communication_config_databit = new System.Windows.Forms.TextBox();
            this.lbl_communication_config_databit = new System.Windows.Forms.Label();
            this.txtbox_communication_config_baud_rate = new System.Windows.Forms.TextBox();
            this.txtbox_communication_config_port = new System.Windows.Forms.TextBox();
            this.lbl_communication_config_baud_rate = new System.Windows.Forms.Label();
            this.lbl_communication_config_port = new System.Windows.Forms.Label();
            this.grp_communication_config.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_communication_config_cancel
            // 
            this.btn_communication_config_cancel.Location = new System.Drawing.Point(138, 208);
            this.btn_communication_config_cancel.Name = "btn_communication_config_cancel";
            this.btn_communication_config_cancel.Size = new System.Drawing.Size(75, 23);
            this.btn_communication_config_cancel.TabIndex = 6;
            this.btn_communication_config_cancel.Text = "取消";
            this.btn_communication_config_cancel.UseVisualStyleBackColor = true;
            this.btn_communication_config_cancel.Click += new System.EventHandler(this.btn_communication_config_cancel_Click);
            // 
            // btn_communication_config_save
            // 
            this.btn_communication_config_save.Location = new System.Drawing.Point(48, 208);
            this.btn_communication_config_save.Name = "btn_communication_config_save";
            this.btn_communication_config_save.Size = new System.Drawing.Size(75, 23);
            this.btn_communication_config_save.TabIndex = 5;
            this.btn_communication_config_save.Text = "保存";
            this.btn_communication_config_save.UseVisualStyleBackColor = true;
            this.btn_communication_config_save.Click += new System.EventHandler(this.btn_communication_config_save_Click);
            // 
            // grp_communication_config
            // 
            this.grp_communication_config.Controls.Add(this.cmb_communication_config_parity);
            this.grp_communication_config.Controls.Add(this.cmb_communication_config_stopbit);
            this.grp_communication_config.Controls.Add(this.lbl_communication_config_parity);
            this.grp_communication_config.Controls.Add(this.lbl_communication_config_stopbit);
            this.grp_communication_config.Controls.Add(this.txtbox_communication_config_databit);
            this.grp_communication_config.Controls.Add(this.lbl_communication_config_databit);
            this.grp_communication_config.Controls.Add(this.txtbox_communication_config_baud_rate);
            this.grp_communication_config.Controls.Add(this.txtbox_communication_config_port);
            this.grp_communication_config.Controls.Add(this.lbl_communication_config_baud_rate);
            this.grp_communication_config.Controls.Add(this.lbl_communication_config_port);
            this.grp_communication_config.Location = new System.Drawing.Point(7, 6);
            this.grp_communication_config.Name = "grp_communication_config";
            this.grp_communication_config.Size = new System.Drawing.Size(243, 193);
            this.grp_communication_config.TabIndex = 8;
            this.grp_communication_config.TabStop = false;
            this.grp_communication_config.Text = "参数";
            // 
            // cmb_communication_config_parity
            // 
            this.cmb_communication_config_parity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_communication_config_parity.FormattingEnabled = true;
            this.cmb_communication_config_parity.Items.AddRange(new object[] {
            "None",
            "Odd",
            "Even",
            "Mark",
            "Space"});
            this.cmb_communication_config_parity.Location = new System.Drawing.Point(86, 120);
            this.cmb_communication_config_parity.Name = "cmb_communication_config_parity";
            this.cmb_communication_config_parity.Size = new System.Drawing.Size(129, 20);
            this.cmb_communication_config_parity.TabIndex = 3;
            // 
            // cmb_communication_config_stopbit
            // 
            this.cmb_communication_config_stopbit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_communication_config_stopbit.FormattingEnabled = true;
            this.cmb_communication_config_stopbit.Items.AddRange(new object[] {
            "One",
            "Two",
            "OnePointFive"});
            this.cmb_communication_config_stopbit.Location = new System.Drawing.Point(86, 158);
            this.cmb_communication_config_stopbit.Name = "cmb_communication_config_stopbit";
            this.cmb_communication_config_stopbit.Size = new System.Drawing.Size(129, 20);
            this.cmb_communication_config_stopbit.TabIndex = 4;
            // 
            // lbl_communication_config_parity
            // 
            this.lbl_communication_config_parity.Location = new System.Drawing.Point(15, 120);
            this.lbl_communication_config_parity.Name = "lbl_communication_config_parity";
            this.lbl_communication_config_parity.Size = new System.Drawing.Size(52, 23);
            this.lbl_communication_config_parity.TabIndex = 7;
            this.lbl_communication_config_parity.Text = "校验位";
            this.lbl_communication_config_parity.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbl_communication_config_stopbit
            // 
            this.lbl_communication_config_stopbit.Location = new System.Drawing.Point(15, 155);
            this.lbl_communication_config_stopbit.Name = "lbl_communication_config_stopbit";
            this.lbl_communication_config_stopbit.Size = new System.Drawing.Size(52, 23);
            this.lbl_communication_config_stopbit.TabIndex = 6;
            this.lbl_communication_config_stopbit.Text = "停止位";
            this.lbl_communication_config_stopbit.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtbox_communication_config_databit
            // 
            this.txtbox_communication_config_databit.Location = new System.Drawing.Point(86, 87);
            this.txtbox_communication_config_databit.Name = "txtbox_communication_config_databit";
            this.txtbox_communication_config_databit.Size = new System.Drawing.Size(129, 21);
            this.txtbox_communication_config_databit.TabIndex = 2;
            // 
            // lbl_communication_config_databit
            // 
            this.lbl_communication_config_databit.Location = new System.Drawing.Point(15, 85);
            this.lbl_communication_config_databit.Name = "lbl_communication_config_databit";
            this.lbl_communication_config_databit.Size = new System.Drawing.Size(52, 23);
            this.lbl_communication_config_databit.TabIndex = 4;
            this.lbl_communication_config_databit.Text = "数据位";
            this.lbl_communication_config_databit.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtbox_communication_config_baud_rate
            // 
            this.txtbox_communication_config_baud_rate.Location = new System.Drawing.Point(86, 50);
            this.txtbox_communication_config_baud_rate.Name = "txtbox_communication_config_baud_rate";
            this.txtbox_communication_config_baud_rate.Size = new System.Drawing.Size(129, 21);
            this.txtbox_communication_config_baud_rate.TabIndex = 1;
            // 
            // txtbox_communication_config_port
            // 
            this.txtbox_communication_config_port.Location = new System.Drawing.Point(86, 15);
            this.txtbox_communication_config_port.Name = "txtbox_communication_config_port";
            this.txtbox_communication_config_port.Size = new System.Drawing.Size(129, 21);
            this.txtbox_communication_config_port.TabIndex = 0;
            // 
            // lbl_communication_config_baud_rate
            // 
            this.lbl_communication_config_baud_rate.Location = new System.Drawing.Point(15, 50);
            this.lbl_communication_config_baud_rate.Name = "lbl_communication_config_baud_rate";
            this.lbl_communication_config_baud_rate.Size = new System.Drawing.Size(52, 23);
            this.lbl_communication_config_baud_rate.TabIndex = 1;
            this.lbl_communication_config_baud_rate.Text = "波特率";
            this.lbl_communication_config_baud_rate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbl_communication_config_port
            // 
            this.lbl_communication_config_port.Location = new System.Drawing.Point(15, 15);
            this.lbl_communication_config_port.Name = "lbl_communication_config_port";
            this.lbl_communication_config_port.Size = new System.Drawing.Size(52, 23);
            this.lbl_communication_config_port.TabIndex = 0;
            this.lbl_communication_config_port.Text = "端口名";
            this.lbl_communication_config_port.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // frm_communication_config
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(258, 239);
            this.ControlBox = false;
            this.Controls.Add(this.btn_communication_config_cancel);
            this.Controls.Add(this.btn_communication_config_save);
            this.Controls.Add(this.grp_communication_config);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frm_communication_config";
            this.Text = "通讯设置";
            this.grp_communication_config.ResumeLayout(false);
            this.grp_communication_config.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_communication_config_cancel;
        private System.Windows.Forms.Button btn_communication_config_save;
        private System.Windows.Forms.GroupBox grp_communication_config;
        public System.Windows.Forms.ComboBox cmb_communication_config_parity;
        public System.Windows.Forms.ComboBox cmb_communication_config_stopbit;
        private System.Windows.Forms.Label lbl_communication_config_parity;
        private System.Windows.Forms.Label lbl_communication_config_stopbit;
        public System.Windows.Forms.TextBox txtbox_communication_config_databit;
        private System.Windows.Forms.Label lbl_communication_config_databit;
        public System.Windows.Forms.TextBox txtbox_communication_config_baud_rate;
        public System.Windows.Forms.TextBox txtbox_communication_config_port;
        private System.Windows.Forms.Label lbl_communication_config_baud_rate;
        private System.Windows.Forms.Label lbl_communication_config_port;
    }
}