namespace Interpolation
{
    partial class ODE
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btnSolve = new System.Windows.Forms.Button();
            this.tabInput = new System.Windows.Forms.TabControl();
            this.tabSystem = new System.Windows.Forms.TabPage();
            this.grpSysInit = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtInitZ = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtInitY = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtInitX = new System.Windows.Forms.TextBox();
            this.grpSysEq = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtFuncZ = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtFuncY = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtFuncX = new System.Windows.Forms.TextBox();
            this.panelSysConfig = new System.Windows.Forms.Panel();
            this.rdoSys3 = new System.Windows.Forms.RadioButton();
            this.rdoSys2 = new System.Windows.Forms.RadioButton();
            this.tabHighOrder = new System.Windows.Forms.TabPage();
            this.grpHighInit = new System.Windows.Forms.GroupBox();
            this.label13 = new System.Windows.Forms.Label();
            this.txtInitD2y = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.txtInitDy = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtInitY_High = new System.Windows.Forms.TextBox();
            this.grpHighEq = new System.Windows.Forms.GroupBox();
            this.lblHighHint = new System.Windows.Forms.Label();
            this.txtFuncHigh = new System.Windows.Forms.TextBox();
            this.lblFuncHigh = new System.Windows.Forms.Label();
            this.panelHighConfig = new System.Windows.Forms.Panel();
            this.label14 = new System.Windows.Forms.Label();
            this.cmbOrder = new System.Windows.Forms.ComboBox();
            this.grpCommon = new System.Windows.Forms.GroupBox();
            this.lblEpsilon = new System.Windows.Forms.Label();
            this.txtEpsilon = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbMethod = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtH = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtTend = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtT0 = new System.Windows.Forms.TextBox();
            this.tabOutput = new System.Windows.Forms.TabControl();
            this.tabTable = new System.Windows.Forms.TabPage();
            this.dgvResult = new System.Windows.Forms.DataGridView();
            this.tabGraph = new System.Windows.Forms.TabPage();
            this.chartResult = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.tabLog = new System.Windows.Forms.TabPage();
            this.rtbLog = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabInput.SuspendLayout();
            this.tabSystem.SuspendLayout();
            this.grpSysInit.SuspendLayout();
            this.grpSysEq.SuspendLayout();
            this.panelSysConfig.SuspendLayout();
            this.tabHighOrder.SuspendLayout();
            this.grpHighInit.SuspendLayout();
            this.grpHighEq.SuspendLayout();
            this.panelHighConfig.SuspendLayout();
            this.grpCommon.SuspendLayout();
            this.tabOutput.SuspendLayout();
            this.tabTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResult)).BeginInit();
            this.tabGraph.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartResult)).BeginInit();
            this.tabLog.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.btnSolve);
            this.splitContainer1.Panel1.Controls.Add(this.tabInput);
            this.splitContainer1.Panel1.Controls.Add(this.grpCommon);
            this.splitContainer1.Panel1.Padding = new System.Windows.Forms.Padding(10);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabOutput);
            this.splitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(10);
            this.splitContainer1.Size = new System.Drawing.Size(1182, 653);
            this.splitContainer1.SplitterDistance = 450;
            this.splitContainer1.TabIndex = 0;
            // 
            // btnSolve
            // 
            this.btnSolve.BackColor = System.Drawing.Color.SteelBlue;
            this.btnSolve.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnSolve.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSolve.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSolve.ForeColor = System.Drawing.Color.White;
            this.btnSolve.Location = new System.Drawing.Point(10, 593);
            this.btnSolve.Name = "btnSolve";
            this.btnSolve.Size = new System.Drawing.Size(430, 50);
            this.btnSolve.TabIndex = 2;
            this.btnSolve.Text = "GIẢI PHƯƠNG TRÌNH";
            this.btnSolve.UseVisualStyleBackColor = false;
            this.btnSolve.Click += new System.EventHandler(this.btnSolve_Click);
            // 
            // tabInput
            // 
            this.tabInput.Controls.Add(this.tabSystem);
            this.tabInput.Controls.Add(this.tabHighOrder);
            this.tabInput.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabInput.Location = new System.Drawing.Point(10, 185);
            this.tabInput.Name = "tabInput";
            this.tabInput.SelectedIndex = 0;
            this.tabInput.Size = new System.Drawing.Size(430, 400);
            this.tabInput.TabIndex = 1;
            // 
            // tabSystem
            // 
            this.tabSystem.Controls.Add(this.grpSysInit);
            this.tabSystem.Controls.Add(this.grpSysEq);
            this.tabSystem.Controls.Add(this.panelSysConfig);
            this.tabSystem.Location = new System.Drawing.Point(4, 25);
            this.tabSystem.Name = "tabSystem";
            this.tabSystem.Padding = new System.Windows.Forms.Padding(3);
            this.tabSystem.Size = new System.Drawing.Size(422, 371);
            this.tabSystem.TabIndex = 0;
            this.tabSystem.Text = "Hệ phương trình";
            this.tabSystem.UseVisualStyleBackColor = true;
            // 
            // grpSysInit
            // 
            this.grpSysInit.Controls.Add(this.label10);
            this.grpSysInit.Controls.Add(this.txtInitZ);
            this.grpSysInit.Controls.Add(this.label9);
            this.grpSysInit.Controls.Add(this.txtInitY);
            this.grpSysInit.Controls.Add(this.label8);
            this.grpSysInit.Controls.Add(this.txtInitX);
            this.grpSysInit.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpSysInit.Location = new System.Drawing.Point(3, 213);
            this.grpSysInit.Name = "grpSysInit";
            this.grpSysInit.Size = new System.Drawing.Size(416, 150);
            this.grpSysInit.TabIndex = 2;
            this.grpSysInit.TabStop = false;
            this.grpSysInit.Text = "Giá trị ban đầu (tại t0)";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(20, 100);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(44, 16);
            this.label10.TabIndex = 5;
            this.label10.Text = "z(t0) = ";
            // 
            // txtInitZ
            // 
            this.txtInitZ.Location = new System.Drawing.Point(80, 97);
            this.txtInitZ.Name = "txtInitZ";
            this.txtInitZ.Size = new System.Drawing.Size(100, 22);
            this.txtInitZ.TabIndex = 4;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(20, 65);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(45, 16);
            this.label9.TabIndex = 3;
            this.label9.Text = "y(t0) = ";
            // 
            // txtInitY
            // 
            this.txtInitY.Location = new System.Drawing.Point(80, 62);
            this.txtInitY.Name = "txtInitY";
            this.txtInitY.Size = new System.Drawing.Size(100, 22);
            this.txtInitY.TabIndex = 2;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(20, 30);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(44, 16);
            this.label8.TabIndex = 1;
            this.label8.Text = "x(t0) = ";
            // 
            // txtInitX
            // 
            this.txtInitX.Location = new System.Drawing.Point(80, 27);
            this.txtInitX.Name = "txtInitX";
            this.txtInitX.Size = new System.Drawing.Size(100, 22);
            this.txtInitX.TabIndex = 0;
            // 
            // grpSysEq
            // 
            this.grpSysEq.Controls.Add(this.label7);
            this.grpSysEq.Controls.Add(this.txtFuncZ);
            this.grpSysEq.Controls.Add(this.label6);
            this.grpSysEq.Controls.Add(this.txtFuncY);
            this.grpSysEq.Controls.Add(this.label5);
            this.grpSysEq.Controls.Add(this.txtFuncX);
            this.grpSysEq.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpSysEq.Location = new System.Drawing.Point(3, 43);
            this.grpSysEq.Name = "grpSysEq";
            this.grpSysEq.Size = new System.Drawing.Size(416, 170);
            this.grpSysEq.TabIndex = 1;
            this.grpSysEq.TabStop = false;
            this.grpSysEq.Text = "Vế phải phương trình";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(15, 110);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(49, 16);
            this.label7.TabIndex = 5;
            this.label7.Text = "z\' = η = ";
            // 
            // txtFuncZ
            // 
            this.txtFuncZ.Location = new System.Drawing.Point(80, 107);
            this.txtFuncZ.Name = "txtFuncZ";
            this.txtFuncZ.Size = new System.Drawing.Size(320, 22);
            this.txtFuncZ.TabIndex = 4;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 70);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(51, 16);
            this.label6.TabIndex = 3;
            this.label6.Text = "y\' = g = ";
            // 
            // txtFuncY
            // 
            this.txtFuncY.Location = new System.Drawing.Point(80, 67);
            this.txtFuncY.Name = "txtFuncY";
            this.txtFuncY.Size = new System.Drawing.Size(320, 22);
            this.txtFuncY.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 30);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(45, 16);
            this.label5.TabIndex = 1;
            this.label5.Text = "x\' = f = ";
            // 
            // txtFuncX
            // 
            this.txtFuncX.Location = new System.Drawing.Point(80, 27);
            this.txtFuncX.Name = "txtFuncX";
            this.txtFuncX.Size = new System.Drawing.Size(320, 22);
            this.txtFuncX.TabIndex = 0;
            // 
            // panelSysConfig
            // 
            this.panelSysConfig.Controls.Add(this.rdoSys3);
            this.panelSysConfig.Controls.Add(this.rdoSys2);
            this.panelSysConfig.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSysConfig.Location = new System.Drawing.Point(3, 3);
            this.panelSysConfig.Name = "panelSysConfig";
            this.panelSysConfig.Size = new System.Drawing.Size(416, 40);
            this.panelSysConfig.TabIndex = 0;
            // 
            // rdoSys3
            // 
            this.rdoSys3.AutoSize = true;
            this.rdoSys3.Location = new System.Drawing.Point(150, 10);
            this.rdoSys3.Name = "rdoSys3";
            this.rdoSys3.Size = new System.Drawing.Size(127, 20);
            this.rdoSys3.TabIndex = 1;
            this.rdoSys3.Text = "Hệ 3 chiều (x,y,z)";
            this.rdoSys3.UseVisualStyleBackColor = true;
            this.rdoSys3.CheckedChanged += new System.EventHandler(this.rdoSys3_CheckedChanged);
            // 
            // rdoSys2
            // 
            this.rdoSys2.AutoSize = true;
            this.rdoSys2.Checked = true;
            this.rdoSys2.Location = new System.Drawing.Point(10, 10);
            this.rdoSys2.Name = "rdoSys2";
            this.rdoSys2.Size = new System.Drawing.Size(118, 20);
            this.rdoSys2.TabIndex = 0;
            this.rdoSys2.TabStop = true;
            this.rdoSys2.Text = "Hệ 2 chiều (x,y)";
            this.rdoSys2.UseVisualStyleBackColor = true;
            this.rdoSys2.CheckedChanged += new System.EventHandler(this.rdoSys2_CheckedChanged);
            // 
            // tabHighOrder
            // 
            this.tabHighOrder.Controls.Add(this.grpHighInit);
            this.tabHighOrder.Controls.Add(this.grpHighEq);
            this.tabHighOrder.Controls.Add(this.panelHighConfig);
            this.tabHighOrder.Location = new System.Drawing.Point(4, 25);
            this.tabHighOrder.Name = "tabHighOrder";
            this.tabHighOrder.Padding = new System.Windows.Forms.Padding(3);
            this.tabHighOrder.Size = new System.Drawing.Size(422, 371);
            this.tabHighOrder.TabIndex = 1;
            this.tabHighOrder.Text = "PT bậc cao";
            this.tabHighOrder.UseVisualStyleBackColor = true;
            // 
            // grpHighInit
            // 
            this.grpHighInit.Controls.Add(this.label13);
            this.grpHighInit.Controls.Add(this.txtInitD2y);
            this.grpHighInit.Controls.Add(this.label12);
            this.grpHighInit.Controls.Add(this.txtInitDy);
            this.grpHighInit.Controls.Add(this.label11);
            this.grpHighInit.Controls.Add(this.txtInitY_High);
            this.grpHighInit.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpHighInit.Location = new System.Drawing.Point(3, 143);
            this.grpHighInit.Name = "grpHighInit";
            this.grpHighInit.Size = new System.Drawing.Size(416, 150);
            this.grpHighInit.TabIndex = 2;
            this.grpHighInit.TabStop = false;
            this.grpHighInit.Text = "Giá trị ban đầu";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(20, 100);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(51, 16);
            this.label13.TabIndex = 5;
            this.label13.Text = "y\'\'(t0) = ";
            // 
            // txtInitD2y
            // 
            this.txtInitD2y.Location = new System.Drawing.Point(90, 97);
            this.txtInitD2y.Name = "txtInitD2y";
            this.txtInitD2y.Size = new System.Drawing.Size(100, 22);
            this.txtInitD2y.TabIndex = 4;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(20, 65);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(48, 16);
            this.label12.TabIndex = 3;
            this.label12.Text = "y\'(t0) = ";
            // 
            // txtInitDy
            // 
            this.txtInitDy.Location = new System.Drawing.Point(90, 62);
            this.txtInitDy.Name = "txtInitDy";
            this.txtInitDy.Size = new System.Drawing.Size(100, 22);
            this.txtInitDy.TabIndex = 2;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(20, 30);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(45, 16);
            this.label11.TabIndex = 1;
            this.label11.Text = "y(t0) = ";
            // 
            // txtInitY_High
            // 
            this.txtInitY_High.Location = new System.Drawing.Point(90, 27);
            this.txtInitY_High.Name = "txtInitY_High";
            this.txtInitY_High.Size = new System.Drawing.Size(100, 22);
            this.txtInitY_High.TabIndex = 0;
            // 
            // grpHighEq
            // 
            this.grpHighEq.Controls.Add(this.lblHighHint);
            this.grpHighEq.Controls.Add(this.txtFuncHigh);
            this.grpHighEq.Controls.Add(this.lblFuncHigh);
            this.grpHighEq.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpHighEq.Location = new System.Drawing.Point(3, 43);
            this.grpHighEq.Name = "grpHighEq";
            this.grpHighEq.Size = new System.Drawing.Size(416, 100);
            this.grpHighEq.TabIndex = 1;
            this.grpHighEq.TabStop = false;
            this.grpHighEq.Text = "Phương trình vi phân";
            // 
            // lblHighHint
            // 
            this.lblHighHint.AutoSize = true;
            this.lblHighHint.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHighHint.ForeColor = System.Drawing.Color.Gray;
            this.lblHighHint.Location = new System.Drawing.Point(80, 55);
            this.lblHighHint.Name = "lblHighHint";
            this.lblHighHint.Size = new System.Drawing.Size(219, 16);
            this.lblHighHint.TabIndex = 2;
            this.lblHighHint.Text = "Dùng: t, y, dy (đạo hàm cấp 1), d2y...";
            // 
            // txtFuncHigh
            // 
            this.txtFuncHigh.Location = new System.Drawing.Point(80, 27);
            this.txtFuncHigh.Name = "txtFuncHigh";
            this.txtFuncHigh.Size = new System.Drawing.Size(320, 22);
            this.txtFuncHigh.TabIndex = 1;
            // 
            // lblFuncHigh
            // 
            this.lblFuncHigh.AutoSize = true;
            this.lblFuncHigh.Location = new System.Drawing.Point(15, 30);
            this.lblFuncHigh.Name = "lblFuncHigh";
            this.lblFuncHigh.Size = new System.Drawing.Size(58, 16);
            this.lblFuncHigh.TabIndex = 0;
            this.lblFuncHigh.Text = "y^(k) = f :";
            // 
            // panelHighConfig
            // 
            this.panelHighConfig.Controls.Add(this.label14);
            this.panelHighConfig.Controls.Add(this.cmbOrder);
            this.panelHighConfig.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHighConfig.Location = new System.Drawing.Point(3, 3);
            this.panelHighConfig.Name = "panelHighConfig";
            this.panelHighConfig.Size = new System.Drawing.Size(416, 40);
            this.panelHighConfig.TabIndex = 0;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(10, 10);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(122, 16);
            this.label14.TabIndex = 1;
            this.label14.Text = "Chọn cấp (Order k):";
            // 
            // cmbOrder
            // 
            this.cmbOrder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbOrder.FormattingEnabled = true;
            this.cmbOrder.Items.AddRange(new object[] {
            "Cấp 1 (y\')",
            "Cấp 2 (y\'\')",
            "Cấp 3 (y\'\'\')"});
            this.cmbOrder.Location = new System.Drawing.Point(150, 7);
            this.cmbOrder.Name = "cmbOrder";
            this.cmbOrder.Size = new System.Drawing.Size(150, 24);
            this.cmbOrder.TabIndex = 0;
            this.cmbOrder.SelectedIndexChanged += new System.EventHandler(this.cmbOrder_SelectedIndexChanged);
            // 
            // grpCommon
            // 
            this.grpCommon.Controls.Add(this.lblEpsilon);
            this.grpCommon.Controls.Add(this.txtEpsilon);
            this.grpCommon.Controls.Add(this.label4);
            this.grpCommon.Controls.Add(this.cmbMethod);
            this.grpCommon.Controls.Add(this.label3);
            this.grpCommon.Controls.Add(this.txtH);
            this.grpCommon.Controls.Add(this.label2);
            this.grpCommon.Controls.Add(this.txtTend);
            this.grpCommon.Controls.Add(this.label1);
            this.grpCommon.Controls.Add(this.txtT0);
            this.grpCommon.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpCommon.Location = new System.Drawing.Point(10, 10);
            this.grpCommon.Name = "grpCommon";
            this.grpCommon.Size = new System.Drawing.Size(430, 175);
            this.grpCommon.TabIndex = 0;
            this.grpCommon.TabStop = false;
            this.grpCommon.Text = "Cấu hình chung";
            // 
            // lblEpsilon
            // 
            this.lblEpsilon.AutoSize = true;
            this.lblEpsilon.Location = new System.Drawing.Point(240, 95);
            this.lblEpsilon.Name = "lblEpsilon";
            this.lblEpsilon.Size = new System.Drawing.Size(55, 16);
            this.lblEpsilon.TabIndex = 8;
            this.lblEpsilon.Text = "Epsilon:";
            // 
            // txtEpsilon
            // 
            this.txtEpsilon.Location = new System.Drawing.Point(300, 92);
            this.txtEpsilon.Name = "txtEpsilon";
            this.txtEpsilon.Size = new System.Drawing.Size(80, 22);
            this.txtEpsilon.TabIndex = 9;
            this.txtEpsilon.Text = "1e-6";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(20, 130);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(90, 16);
            this.label4.TabIndex = 7;
            this.label4.Text = "Phương pháp:";
            // 
            // cmbMethod
            // 
            this.cmbMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMethod.FormattingEnabled = true;
            this.cmbMethod.Items.AddRange(new object[] {
            "Euler hiện",
            "Euler ẩn",
            "Hình thang"});
            this.cmbMethod.Location = new System.Drawing.Point(130, 127);
            this.cmbMethod.Name = "cmbMethod";
            this.cmbMethod.Size = new System.Drawing.Size(200, 24);
            this.cmbMethod.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 95);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 16);
            this.label3.TabIndex = 5;
            this.label3.Text = "Bước nhảy h:";
            // 
            // txtH
            // 
            this.txtH.Location = new System.Drawing.Point(130, 92);
            this.txtH.Name = "txtH";
            this.txtH.Size = new System.Drawing.Size(100, 22);
            this.txtH.TabIndex = 4;
            this.txtH.Text = "0.1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(252, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(19, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "T:";
            // 
            // txtTend
            // 
            this.txtTend.Location = new System.Drawing.Point(280, 57);
            this.txtTend.Name = "txtTend";
            this.txtTend.Size = new System.Drawing.Size(100, 22);
            this.txtTend.TabIndex = 2;
            this.txtTend.Text = "1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(20, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "t0:";
            // 
            // txtT0
            // 
            this.txtT0.Location = new System.Drawing.Point(130, 57);
            this.txtT0.Name = "txtT0";
            this.txtT0.Size = new System.Drawing.Size(50, 22);
            this.txtT0.TabIndex = 0;
            this.txtT0.Text = "0";
            // 
            // tabOutput
            // 
            this.tabOutput.Controls.Add(this.tabTable);
            this.tabOutput.Controls.Add(this.tabGraph);
            this.tabOutput.Controls.Add(this.tabLog);
            this.tabOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabOutput.Location = new System.Drawing.Point(10, 10);
            this.tabOutput.Name = "tabOutput";
            this.tabOutput.SelectedIndex = 0;
            this.tabOutput.Size = new System.Drawing.Size(708, 633);
            this.tabOutput.TabIndex = 0;
            // 
            // tabTable
            // 
            this.tabTable.Controls.Add(this.dgvResult);
            this.tabTable.Location = new System.Drawing.Point(4, 25);
            this.tabTable.Name = "tabTable";
            this.tabTable.Padding = new System.Windows.Forms.Padding(3);
            this.tabTable.Size = new System.Drawing.Size(700, 604);
            this.tabTable.TabIndex = 0;
            this.tabTable.Text = "Bảng kết quả";
            this.tabTable.UseVisualStyleBackColor = true;
            // 
            // dgvResult
            // 
            this.dgvResult.AllowUserToAddRows = false;
            this.dgvResult.AllowUserToDeleteRows = false;
            this.dgvResult.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvResult.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvResult.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgvResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvResult.Location = new System.Drawing.Point(3, 3);
            this.dgvResult.Name = "dgvResult";
            this.dgvResult.ReadOnly = true;
            this.dgvResult.RowHeadersWidth = 51;
            this.dgvResult.RowTemplate.Height = 24;
            this.dgvResult.Size = new System.Drawing.Size(694, 598);
            this.dgvResult.TabIndex = 0;
            // 
            // tabGraph
            // 
            this.tabGraph.Controls.Add(this.chartResult);
            this.tabGraph.Location = new System.Drawing.Point(4, 25);
            this.tabGraph.Name = "tabGraph";
            this.tabGraph.Padding = new System.Windows.Forms.Padding(3);
            this.tabGraph.Size = new System.Drawing.Size(700, 604);
            this.tabGraph.TabIndex = 2;
            this.tabGraph.Text = "Đồ thị nghiệm";
            this.tabGraph.UseVisualStyleBackColor = true;
            // 
            // chartResult
            // 
            chartArea1.Name = "ChartArea1";
            this.chartResult.ChartAreas.Add(chartArea1);
            this.chartResult.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Name = "Legend1";
            this.chartResult.Legends.Add(legend1);
            this.chartResult.Location = new System.Drawing.Point(3, 3);
            this.chartResult.Name = "chartResult";
            this.chartResult.Size = new System.Drawing.Size(694, 598);
            this.chartResult.TabIndex = 0;
            this.chartResult.Text = "chart1";
            // 
            // tabLog
            // 
            this.tabLog.Controls.Add(this.rtbLog);
            this.tabLog.Location = new System.Drawing.Point(4, 25);
            this.tabLog.Name = "tabLog";
            this.tabLog.Padding = new System.Windows.Forms.Padding(3);
            this.tabLog.Size = new System.Drawing.Size(700, 604);
            this.tabLog.TabIndex = 1;
            this.tabLog.Text = "Log / Chi tiết";
            this.tabLog.UseVisualStyleBackColor = true;
            // 
            // rtbLog
            // 
            this.rtbLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbLog.Font = new System.Drawing.Font("Consolas", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbLog.Location = new System.Drawing.Point(3, 3);
            this.rtbLog.Name = "rtbLog";
            this.rtbLog.Size = new System.Drawing.Size(694, 598);
            this.rtbLog.TabIndex = 0;
            this.rtbLog.Text = "";
            // 
            // ODE
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1182, 653);
            this.Controls.Add(this.splitContainer1);
            this.Name = "ODE";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Giải phương trình vi phân thường (ODE)";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabInput.ResumeLayout(false);
            this.tabSystem.ResumeLayout(false);
            this.grpSysInit.ResumeLayout(false);
            this.grpSysInit.PerformLayout();
            this.grpSysEq.ResumeLayout(false);
            this.grpSysEq.PerformLayout();
            this.panelSysConfig.ResumeLayout(false);
            this.panelSysConfig.PerformLayout();
            this.tabHighOrder.ResumeLayout(false);
            this.grpHighInit.ResumeLayout(false);
            this.grpHighInit.PerformLayout();
            this.grpHighEq.ResumeLayout(false);
            this.grpHighEq.PerformLayout();
            this.panelHighConfig.ResumeLayout(false);
            this.panelHighConfig.PerformLayout();
            this.grpCommon.ResumeLayout(false);
            this.grpCommon.PerformLayout();
            this.tabOutput.ResumeLayout(false);
            this.tabTable.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvResult)).EndInit();
            this.tabGraph.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chartResult)).EndInit();
            this.tabLog.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox grpCommon;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtT0;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtTend;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtH;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbMethod;
        private System.Windows.Forms.Label lblEpsilon;
        private System.Windows.Forms.TextBox txtEpsilon;
        private System.Windows.Forms.TabControl tabInput;
        private System.Windows.Forms.TabPage tabSystem;
        private System.Windows.Forms.TabPage tabHighOrder;
        private System.Windows.Forms.Panel panelSysConfig;
        private System.Windows.Forms.RadioButton rdoSys3;
        private System.Windows.Forms.RadioButton rdoSys2;
        private System.Windows.Forms.GroupBox grpSysEq;
        private System.Windows.Forms.TextBox txtFuncX;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtFuncZ;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtFuncY;
        private System.Windows.Forms.GroupBox grpSysInit;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtInitZ;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtInitY;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtInitX;
        private System.Windows.Forms.Panel panelHighConfig;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.ComboBox cmbOrder;
        private System.Windows.Forms.GroupBox grpHighEq;
        private System.Windows.Forms.Label lblFuncHigh;
        private System.Windows.Forms.TextBox txtFuncHigh;
        private System.Windows.Forms.Label lblHighHint;
        private System.Windows.Forms.GroupBox grpHighInit;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtInitD2y;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtInitDy;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtInitY_High;
        private System.Windows.Forms.Button btnSolve;
        private System.Windows.Forms.TabControl tabOutput;
        private System.Windows.Forms.TabPage tabTable;
        private System.Windows.Forms.DataGridView dgvResult;
        private System.Windows.Forms.TabPage tabLog;
        private System.Windows.Forms.RichTextBox rtbLog;
        private System.Windows.Forms.TabPage tabGraph;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartResult;
    }
}