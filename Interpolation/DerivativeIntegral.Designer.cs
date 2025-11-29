namespace Interpolation
{
    partial class DerivativeIntegral
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.rtbDerivativeResult = new System.Windows.Forms.RichTextBox();
            this.dataXYDerivative = new System.Windows.Forms.DataGridView();
            this.colsXDerivative = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colsYDerivative = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnDerivative = new System.Windows.Forms.Button();
            this.btnOpenExcelDerivative = new System.Windows.Forms.Button();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.txtBoxXDerivative = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxDerivative = new System.Windows.Forms.ComboBox();
            this.txtBoxPrecisionDerivative = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.grpResult = new System.Windows.Forms.GroupBox();
            this.lblIntegralResult = new System.Windows.Forms.Label();
            this.rtbIntegralResult = new System.Windows.Forms.RichTextBox();
            this.btnCalculateIntegral = new System.Windows.Forms.Button();
            this.grpConfiguration = new System.Windows.Forms.GroupBox();
            this.txtEpsilon = new System.Windows.Forms.TextBox();
            this.lblEpsilon = new System.Windows.Forms.Label();
            this.txtN = new System.Windows.Forms.TextBox();
            this.lblN = new System.Windows.Forms.Label();
            this.rdoCalculateN = new System.Windows.Forms.RadioButton();
            this.rdoFixedN = new System.Windows.Forms.RadioButton();
            this.grpMethod = new System.Windows.Forms.GroupBox();
            this.cmbMethod = new System.Windows.Forms.ComboBox();
            this.lblMethod = new System.Windows.Forms.Label();
            this.panelData = new System.Windows.Forms.Panel();
            this.txtDataEpsilon = new System.Windows.Forms.TextBox();
            this.lblDataEpsilon = new System.Windows.Forms.Label();
            this.txtDataUpperBound = new System.Windows.Forms.TextBox();
            this.lblDataUpperBound = new System.Windows.Forms.Label();
            this.txtDataLowerBound = new System.Windows.Forms.TextBox();
            this.lblDataLowerBound = new System.Windows.Forms.Label();
            this.dgvIntegralData = new System.Windows.Forms.DataGridView();
            this.colsXIntegral = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colsYIntegral = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnClearDataIntegral = new System.Windows.Forms.Button();
            this.btnOpenExcelIntegral = new System.Windows.Forms.Button();
            this.panelFunction = new System.Windows.Forms.Panel();
            this.txtUpperBound = new System.Windows.Forms.TextBox();
            this.lblUpperBound = new System.Windows.Forms.Label();
            this.txtLowerBound = new System.Windows.Forms.TextBox();
            this.lblLowerBound = new System.Windows.Forms.Label();
            this.lblFunctionHint = new System.Windows.Forms.Label();
            this.txtFunction = new System.Windows.Forms.TextBox();
            this.lblFunction = new System.Windows.Forms.Label();
            this.grpInputType = new System.Windows.Forms.GroupBox();
            this.rdoData = new System.Windows.Forms.RadioButton();
            this.rdoFunction = new System.Windows.Forms.RadioButton();
            this.txtBoxFunction = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataXYDerivative)).BeginInit();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.grpResult.SuspendLayout();
            this.grpConfiguration.SuspendLayout();
            this.grpMethod.SuspendLayout();
            this.panelData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvIntegralData)).BeginInit();
            this.panelFunction.SuspendLayout();
            this.grpInputType.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1224, 615);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.rtbDerivativeResult);
            this.tabPage1.Controls.Add(this.dataXYDerivative);
            this.tabPage1.Controls.Add(this.panel1);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1216, 586);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Đạo hàm";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // rtbDerivativeResult
            // 
            this.rtbDerivativeResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbDerivativeResult.Font = new System.Drawing.Font("Consolas", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbDerivativeResult.Location = new System.Drawing.Point(256, 3);
            this.rtbDerivativeResult.Name = "rtbDerivativeResult";
            this.rtbDerivativeResult.Size = new System.Drawing.Size(957, 352);
            this.rtbDerivativeResult.TabIndex = 2;
            this.rtbDerivativeResult.Text = "";
            // 
            // dataXYDerivative
            // 
            this.dataXYDerivative.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataXYDerivative.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colsXDerivative,
            this.colsYDerivative});
            this.dataXYDerivative.Dock = System.Windows.Forms.DockStyle.Left;
            this.dataXYDerivative.Location = new System.Drawing.Point(3, 3);
            this.dataXYDerivative.Name = "dataXYDerivative";
            this.dataXYDerivative.RowHeadersWidth = 51;
            this.dataXYDerivative.RowTemplate.Height = 24;
            this.dataXYDerivative.Size = new System.Drawing.Size(253, 352);
            this.dataXYDerivative.TabIndex = 1;
            // 
            // colsXDerivative
            // 
            this.colsXDerivative.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colsXDerivative.HeaderText = "X";
            this.colsXDerivative.MinimumWidth = 6;
            this.colsXDerivative.Name = "colsXDerivative";
            // 
            // colsYDerivative
            // 
            this.colsYDerivative.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colsYDerivative.HeaderText = "Y";
            this.colsYDerivative.MinimumWidth = 6;
            this.colsYDerivative.Name = "colsYDerivative";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tableLayoutPanel1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(3, 355);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1210, 228);
            this.panel1.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.btnDerivative, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnOpenExcelDerivative, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 49.12281F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.87719F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1210, 228);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // btnDerivative
            // 
            this.btnDerivative.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnDerivative.AutoSize = true;
            this.btnDerivative.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDerivative.Location = new System.Drawing.Point(820, 42);
            this.btnDerivative.Name = "btnDerivative";
            this.btnDerivative.Size = new System.Drawing.Size(175, 28);
            this.btnDerivative.TabIndex = 3;
            this.btnDerivative.Text = "Tính giá trị đạo hàm";
            this.btnDerivative.UseVisualStyleBackColor = true;
            this.btnDerivative.Click += new System.EventHandler(this.btnDerivative_Click);
            // 
            // btnOpenExcelDerivative
            // 
            this.btnOpenExcelDerivative.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnOpenExcelDerivative.AutoSize = true;
            this.btnOpenExcelDerivative.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOpenExcelDerivative.Location = new System.Drawing.Point(841, 156);
            this.btnOpenExcelDerivative.Name = "btnOpenExcelDerivative";
            this.btnOpenExcelDerivative.Size = new System.Drawing.Size(133, 28);
            this.btnOpenExcelDerivative.TabIndex = 4;
            this.btnOpenExcelDerivative.Text = "Nhập dữ liệu ";
            this.btnOpenExcelDerivative.UseVisualStyleBackColor = true;
            this.btnOpenExcelDerivative.Click += new System.EventHandler(this.btnOpenExcelDerivative_Click);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.label3, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.txtBoxXDerivative, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.txtBoxFunction, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.label2, 1, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 52F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(599, 106);
            this.tableLayoutPanel2.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(302, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(132, 16);
            this.label3.TabIndex = 3;
            this.label3.Text = "Nhập giá trị x cần tính";
            // 
            // txtBoxXDerivative
            // 
            this.txtBoxXDerivative.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.txtBoxXDerivative.Location = new System.Drawing.Point(196, 16);
            this.txtBoxXDerivative.Name = "txtBoxXDerivative";
            this.txtBoxXDerivative.Size = new System.Drawing.Size(100, 22);
            this.txtBoxXDerivative.TabIndex = 1;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Controls.Add(this.label4, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.label1, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.comboBoxDerivative, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.txtBoxPrecisionDerivative, 0, 1);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 115);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 62F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(599, 110);
            this.tableLayoutPanel3.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(302, 71);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(161, 16);
            this.label4.TabIndex = 3;
            this.label4.Text = "Số chữ số phần thập phân";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(302, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Chọn công thức";
            // 
            // comboBoxDerivative
            // 
            this.comboBoxDerivative.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.comboBoxDerivative.FormattingEnabled = true;
            this.comboBoxDerivative.Items.AddRange(new object[] {
            "Công thức 2 điểm",
            "Công thức 3 điểm",
            "Công thức 4 điểm"});
            this.comboBoxDerivative.Location = new System.Drawing.Point(150, 12);
            this.comboBoxDerivative.Name = "comboBoxDerivative";
            this.comboBoxDerivative.Size = new System.Drawing.Size(146, 24);
            this.comboBoxDerivative.TabIndex = 0;
            // 
            // txtBoxPrecisionDerivative
            // 
            this.txtBoxPrecisionDerivative.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.txtBoxPrecisionDerivative.Location = new System.Drawing.Point(196, 68);
            this.txtBoxPrecisionDerivative.Name = "txtBoxPrecisionDerivative";
            this.txtBoxPrecisionDerivative.Size = new System.Drawing.Size(100, 22);
            this.txtBoxPrecisionDerivative.TabIndex = 2;
            this.txtBoxPrecisionDerivative.Text = "7";
            // 
            // tabPage2
            // 
            this.tabPage2.AutoScroll = true;
            this.tabPage2.Controls.Add(this.grpResult);
            this.tabPage2.Controls.Add(this.btnCalculateIntegral);
            this.tabPage2.Controls.Add(this.grpConfiguration);
            this.tabPage2.Controls.Add(this.grpMethod);
            this.tabPage2.Controls.Add(this.panelData);
            this.tabPage2.Controls.Add(this.panelFunction);
            this.tabPage2.Controls.Add(this.grpInputType);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1216, 586);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Tích phân";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // grpResult
            // 
            this.grpResult.Controls.Add(this.lblIntegralResult);
            this.grpResult.Controls.Add(this.rtbIntegralResult);
            this.grpResult.Location = new System.Drawing.Point(20, 680);
            this.grpResult.Name = "grpResult";
            this.grpResult.Size = new System.Drawing.Size(1150, 400);
            this.grpResult.TabIndex = 6;
            this.grpResult.TabStop = false;
            this.grpResult.Text = "Kết quả";
            // 
            // lblIntegralResult
            // 
            this.lblIntegralResult.AutoSize = true;
            this.lblIntegralResult.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lblIntegralResult.ForeColor = System.Drawing.Color.Green;
            this.lblIntegralResult.Location = new System.Drawing.Point(20, 365);
            this.lblIntegralResult.Name = "lblIntegralResult";
            this.lblIntegralResult.Size = new System.Drawing.Size(0, 20);
            this.lblIntegralResult.TabIndex = 1;
            this.lblIntegralResult.Visible = false;
            // 
            // rtbIntegralResult
            // 
            this.rtbIntegralResult.Font = new System.Drawing.Font("Consolas", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbIntegralResult.Location = new System.Drawing.Point(20, 25);
            this.rtbIntegralResult.Name = "rtbIntegralResult";
            this.rtbIntegralResult.ReadOnly = true;
            this.rtbIntegralResult.Size = new System.Drawing.Size(1110, 330);
            this.rtbIntegralResult.TabIndex = 0;
            this.rtbIntegralResult.Text = "";
            // 
            // btnCalculateIntegral
            // 
            this.btnCalculateIntegral.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.btnCalculateIntegral.Location = new System.Drawing.Point(450, 630);
            this.btnCalculateIntegral.Name = "btnCalculateIntegral";
            this.btnCalculateIntegral.Size = new System.Drawing.Size(280, 40);
            this.btnCalculateIntegral.TabIndex = 5;
            this.btnCalculateIntegral.Text = "Tính tích phân";
            this.btnCalculateIntegral.UseVisualStyleBackColor = true;
            this.btnCalculateIntegral.Click += new System.EventHandler(this.btnCalculateIntegral_Click);
            // 
            // grpConfiguration
            // 
            this.grpConfiguration.Controls.Add(this.txtEpsilon);
            this.grpConfiguration.Controls.Add(this.lblEpsilon);
            this.grpConfiguration.Controls.Add(this.txtN);
            this.grpConfiguration.Controls.Add(this.lblN);
            this.grpConfiguration.Controls.Add(this.rdoCalculateN);
            this.grpConfiguration.Controls.Add(this.rdoFixedN);
            this.grpConfiguration.Location = new System.Drawing.Point(20, 470);
            this.grpConfiguration.Name = "grpConfiguration";
            this.grpConfiguration.Size = new System.Drawing.Size(1150, 150);
            this.grpConfiguration.TabIndex = 4;
            this.grpConfiguration.TabStop = false;
            this.grpConfiguration.Text = "Cấu hình tính toán";
            // 
            // txtEpsilon
            // 
            this.txtEpsilon.Enabled = false;
            this.txtEpsilon.Location = new System.Drawing.Point(170, 97);
            this.txtEpsilon.Name = "txtEpsilon";
            this.txtEpsilon.Size = new System.Drawing.Size(100, 22);
            this.txtEpsilon.TabIndex = 5;
            this.txtEpsilon.Text = "0.001";
            // 
            // lblEpsilon
            // 
            this.lblEpsilon.AutoSize = true;
            this.lblEpsilon.Location = new System.Drawing.Point(40, 100);
            this.lblEpsilon.Name = "lblEpsilon";
            this.lblEpsilon.Size = new System.Drawing.Size(124, 16);
            this.lblEpsilon.TabIndex = 4;
            this.lblEpsilon.Text = "Sai số cho phép ε =";
            // 
            // txtN
            // 
            this.txtN.Location = new System.Drawing.Point(70, 47);
            this.txtN.Name = "txtN";
            this.txtN.Size = new System.Drawing.Size(100, 22);
            this.txtN.TabIndex = 3;
            this.txtN.Text = "20";
            // 
            // lblN
            // 
            this.lblN.AutoSize = true;
            this.lblN.Location = new System.Drawing.Point(40, 50);
            this.lblN.Name = "lblN";
            this.lblN.Size = new System.Drawing.Size(24, 16);
            this.lblN.TabIndex = 2;
            this.lblN.Text = "n =";
            // 
            // rdoCalculateN
            // 
            this.rdoCalculateN.AutoSize = true;
            this.rdoCalculateN.Location = new System.Drawing.Point(20, 75);
            this.rdoCalculateN.Name = "rdoCalculateN";
            this.rdoCalculateN.Size = new System.Drawing.Size(116, 20);
            this.rdoCalculateN.TabIndex = 1;
            this.rdoCalculateN.Text = "Tính n từ sai số";
            this.rdoCalculateN.UseVisualStyleBackColor = true;
            // 
            // rdoFixedN
            // 
            this.rdoFixedN.AutoSize = true;
            this.rdoFixedN.Checked = true;
            this.rdoFixedN.Location = new System.Drawing.Point(20, 26);
            this.rdoFixedN.Name = "rdoFixedN";
            this.rdoFixedN.Size = new System.Drawing.Size(188, 20);
            this.rdoFixedN.TabIndex = 0;
            this.rdoFixedN.TabStop = true;
            this.rdoFixedN.Text = "Cho trước số khoảng chia n";
            this.rdoFixedN.UseVisualStyleBackColor = true;
            this.rdoFixedN.CheckedChanged += new System.EventHandler(this.rdoFixedN_CheckedChanged);
            // 
            // grpMethod
            // 
            this.grpMethod.Controls.Add(this.cmbMethod);
            this.grpMethod.Controls.Add(this.lblMethod);
            this.grpMethod.Location = new System.Drawing.Point(20, 400);
            this.grpMethod.Name = "grpMethod";
            this.grpMethod.Size = new System.Drawing.Size(1150, 70);
            this.grpMethod.TabIndex = 3;
            this.grpMethod.TabStop = false;
            this.grpMethod.Text = "Phương pháp tính";
            // 
            // cmbMethod
            // 
            this.cmbMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMethod.FormattingEnabled = true;
            this.cmbMethod.Items.AddRange(new object[] {
            "Hình thang",
            "Simpson"});
            this.cmbMethod.Location = new System.Drawing.Point(160, 27);
            this.cmbMethod.Name = "cmbMethod";
            this.cmbMethod.Size = new System.Drawing.Size(250, 24);
            this.cmbMethod.TabIndex = 1;
            // 
            // lblMethod
            // 
            this.lblMethod.AutoSize = true;
            this.lblMethod.Location = new System.Drawing.Point(20, 30);
            this.lblMethod.Name = "lblMethod";
            this.lblMethod.Size = new System.Drawing.Size(123, 16);
            this.lblMethod.TabIndex = 0;
            this.lblMethod.Text = "Chọn phương pháp:";
            // 
            // panelData
            // 
            this.panelData.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelData.Controls.Add(this.txtDataEpsilon);
            this.panelData.Controls.Add(this.lblDataEpsilon);
            this.panelData.Controls.Add(this.txtDataUpperBound);
            this.panelData.Controls.Add(this.lblDataUpperBound);
            this.panelData.Controls.Add(this.txtDataLowerBound);
            this.panelData.Controls.Add(this.lblDataLowerBound);
            this.panelData.Controls.Add(this.dgvIntegralData);
            this.panelData.Controls.Add(this.btnClearDataIntegral);
            this.panelData.Controls.Add(this.btnOpenExcelIntegral);
            this.panelData.Location = new System.Drawing.Point(20, 100);
            this.panelData.Name = "panelData";
            this.panelData.Size = new System.Drawing.Size(1150, 280);
            this.panelData.TabIndex = 2;
            this.panelData.Visible = false;
            // 
            // txtDataEpsilon
            // 
            this.txtDataEpsilon.Location = new System.Drawing.Point(480, 237);
            this.txtDataEpsilon.Name = "txtDataEpsilon";
            this.txtDataEpsilon.Size = new System.Drawing.Size(100, 22);
            this.txtDataEpsilon.TabIndex = 8;
            this.txtDataEpsilon.Text = "0.001";
            // 
            // lblDataEpsilon
            // 
            this.lblDataEpsilon.AutoSize = true;
            this.lblDataEpsilon.Location = new System.Drawing.Point(420, 240);
            this.lblDataEpsilon.Name = "lblDataEpsilon";
            this.lblDataEpsilon.Size = new System.Drawing.Size(58, 16);
            this.lblDataEpsilon.TabIndex = 7;
            this.lblDataEpsilon.Text = "Sai số ε:";
            // 
            // txtDataUpperBound
            // 
            this.txtDataUpperBound.Location = new System.Drawing.Point(295, 237);
            this.txtDataUpperBound.Name = "txtDataUpperBound";
            this.txtDataUpperBound.Size = new System.Drawing.Size(100, 22);
            this.txtDataUpperBound.TabIndex = 6;
            // 
            // lblDataUpperBound
            // 
            this.lblDataUpperBound.AutoSize = true;
            this.lblDataUpperBound.Location = new System.Drawing.Point(220, 240);
            this.lblDataUpperBound.Name = "lblDataUpperBound";
            this.lblDataUpperBound.Size = new System.Drawing.Size(70, 16);
            this.lblDataUpperBound.TabIndex = 5;
            this.lblDataUpperBound.Text = "Cận trên b:";
            // 
            // txtDataLowerBound
            // 
            this.txtDataLowerBound.Location = new System.Drawing.Point(100, 237);
            this.txtDataLowerBound.Name = "txtDataLowerBound";
            this.txtDataLowerBound.Size = new System.Drawing.Size(100, 22);
            this.txtDataLowerBound.TabIndex = 4;
            // 
            // lblDataLowerBound
            // 
            this.lblDataLowerBound.AutoSize = true;
            this.lblDataLowerBound.Location = new System.Drawing.Point(20, 240);
            this.lblDataLowerBound.Name = "lblDataLowerBound";
            this.lblDataLowerBound.Size = new System.Drawing.Size(74, 16);
            this.lblDataLowerBound.TabIndex = 3;
            this.lblDataLowerBound.Text = "Cận dưới a:";
            // 
            // dgvIntegralData
            // 
            this.dgvIntegralData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvIntegralData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colsXIntegral,
            this.colsYIntegral});
            this.dgvIntegralData.Location = new System.Drawing.Point(20, 60);
            this.dgvIntegralData.Name = "dgvIntegralData";
            this.dgvIntegralData.RowHeadersWidth = 51;
            this.dgvIntegralData.Size = new System.Drawing.Size(359, 150);
            this.dgvIntegralData.TabIndex = 2;
            // 
            // colsXIntegral
            // 
            this.colsXIntegral.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colsXIntegral.HeaderText = "X";
            this.colsXIntegral.MinimumWidth = 6;
            this.colsXIntegral.Name = "colsXIntegral";
            // 
            // colsYIntegral
            // 
            this.colsYIntegral.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colsYIntegral.HeaderText = "Y";
            this.colsYIntegral.MinimumWidth = 6;
            this.colsYIntegral.Name = "colsYIntegral";
            // 
            // btnClearDataIntegral
            // 
            this.btnClearDataIntegral.Location = new System.Drawing.Point(130, 20);
            this.btnClearDataIntegral.Name = "btnClearDataIntegral";
            this.btnClearDataIntegral.Size = new System.Drawing.Size(100, 30);
            this.btnClearDataIntegral.TabIndex = 1;
            this.btnClearDataIntegral.Text = "Xóa dữ liệu";
            this.btnClearDataIntegral.UseVisualStyleBackColor = true;
            this.btnClearDataIntegral.Click += new System.EventHandler(this.btnClearDataIntegral_Click);
            // 
            // btnOpenExcelIntegral
            // 
            this.btnOpenExcelIntegral.Location = new System.Drawing.Point(20, 20);
            this.btnOpenExcelIntegral.Name = "btnOpenExcelIntegral";
            this.btnOpenExcelIntegral.Size = new System.Drawing.Size(100, 30);
            this.btnOpenExcelIntegral.TabIndex = 0;
            this.btnOpenExcelIntegral.Text = "Mở Excel";
            this.btnOpenExcelIntegral.UseVisualStyleBackColor = true;
            this.btnOpenExcelIntegral.Click += new System.EventHandler(this.btnOpenExcelIntegral_Click);
            // 
            // panelFunction
            // 
            this.panelFunction.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelFunction.Controls.Add(this.txtUpperBound);
            this.panelFunction.Controls.Add(this.lblUpperBound);
            this.panelFunction.Controls.Add(this.txtLowerBound);
            this.panelFunction.Controls.Add(this.lblLowerBound);
            this.panelFunction.Controls.Add(this.lblFunctionHint);
            this.panelFunction.Controls.Add(this.txtFunction);
            this.panelFunction.Controls.Add(this.lblFunction);
            this.panelFunction.Location = new System.Drawing.Point(20, 100);
            this.panelFunction.Name = "panelFunction";
            this.panelFunction.Size = new System.Drawing.Size(1150, 120);
            this.panelFunction.TabIndex = 1;
            // 
            // txtUpperBound
            // 
            this.txtUpperBound.Location = new System.Drawing.Point(330, 72);
            this.txtUpperBound.Name = "txtUpperBound";
            this.txtUpperBound.Size = new System.Drawing.Size(120, 22);
            this.txtUpperBound.TabIndex = 6;
            // 
            // lblUpperBound
            // 
            this.lblUpperBound.AutoSize = true;
            this.lblUpperBound.Location = new System.Drawing.Point(250, 75);
            this.lblUpperBound.Name = "lblUpperBound";
            this.lblUpperBound.Size = new System.Drawing.Size(70, 16);
            this.lblUpperBound.TabIndex = 5;
            this.lblUpperBound.Text = "Cận trên b:";
            // 
            // txtLowerBound
            // 
            this.txtLowerBound.Location = new System.Drawing.Point(100, 72);
            this.txtLowerBound.Name = "txtLowerBound";
            this.txtLowerBound.Size = new System.Drawing.Size(120, 22);
            this.txtLowerBound.TabIndex = 4;
            // 
            // lblLowerBound
            // 
            this.lblLowerBound.AutoSize = true;
            this.lblLowerBound.Location = new System.Drawing.Point(20, 75);
            this.lblLowerBound.Name = "lblLowerBound";
            this.lblLowerBound.Size = new System.Drawing.Size(74, 16);
            this.lblLowerBound.TabIndex = 3;
            this.lblLowerBound.Text = "Cận dưới a:";
            // 
            // lblFunctionHint
            // 
            this.lblFunctionHint.AutoSize = true;
            this.lblFunctionHint.ForeColor = System.Drawing.Color.Gray;
            this.lblFunctionHint.Location = new System.Drawing.Point(100, 42);
            this.lblFunctionHint.Name = "lblFunctionHint";
            this.lblFunctionHint.Size = new System.Drawing.Size(234, 16);
            this.lblFunctionHint.TabIndex = 2;
            this.lblFunctionHint.Text = "(Ví dụ: x^2, sin(x), e^x, x^2+2*x, log(x)...)";
            // 
            // txtFunction
            // 
            this.txtFunction.Location = new System.Drawing.Point(100, 17);
            this.txtFunction.Name = "txtFunction";
            this.txtFunction.Size = new System.Drawing.Size(400, 22);
            this.txtFunction.TabIndex = 1;
            // 
            // lblFunction
            // 
            this.lblFunction.AutoSize = true;
            this.lblFunction.Location = new System.Drawing.Point(20, 20);
            this.lblFunction.Name = "lblFunction";
            this.lblFunction.Size = new System.Drawing.Size(59, 16);
            this.lblFunction.TabIndex = 0;
            this.lblFunction.Text = "Hàm f(x):";
            // 
            // grpInputType
            // 
            this.grpInputType.Controls.Add(this.rdoData);
            this.grpInputType.Controls.Add(this.rdoFunction);
            this.grpInputType.Location = new System.Drawing.Point(20, 20);
            this.grpInputType.Name = "grpInputType";
            this.grpInputType.Size = new System.Drawing.Size(1150, 60);
            this.grpInputType.TabIndex = 0;
            this.grpInputType.TabStop = false;
            this.grpInputType.Text = "Loại dữ liệu đầu vào";
            // 
            // rdoData
            // 
            this.rdoData.AutoSize = true;
            this.rdoData.Location = new System.Drawing.Point(200, 25);
            this.rdoData.Name = "rdoData";
            this.rdoData.Size = new System.Drawing.Size(139, 20);
            this.rdoData.TabIndex = 1;
            this.rdoData.Text = "Dữ liệu rời rạc (x, y)";
            this.rdoData.UseVisualStyleBackColor = true;
            // 
            // rdoFunction
            // 
            this.rdoFunction.AutoSize = true;
            this.rdoFunction.Checked = true;
            this.rdoFunction.Location = new System.Drawing.Point(20, 25);
            this.rdoFunction.Name = "rdoFunction";
            this.rdoFunction.Size = new System.Drawing.Size(95, 20);
            this.rdoFunction.TabIndex = 0;
            this.rdoFunction.TabStop = true;
            this.rdoFunction.Text = "Hàm số f(x)";
            this.rdoFunction.UseVisualStyleBackColor = true;
            this.rdoFunction.CheckedChanged += new System.EventHandler(this.rdoFunction_CheckedChanged);
            // 
            // txtBoxFunction
            // 
            this.txtBoxFunction.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.txtBoxFunction.Location = new System.Drawing.Point(196, 69);
            this.txtBoxFunction.Name = "txtBoxFunction";
            this.txtBoxFunction.Size = new System.Drawing.Size(100, 22);
            this.txtBoxFunction.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(302, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(112, 16);
            this.label2.TabIndex = 5;
            this.label2.Text = "Nhập hàm số F(x)";
            // 
            // DerivativeIntegral
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1224, 615);
            this.Controls.Add(this.tabControl1);
            this.Name = "DerivativeIntegral";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Đạo hàm và Tích phân";
            this.Load += new System.EventHandler(this.DerivativeIntegral_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataXYDerivative)).EndInit();
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.grpResult.ResumeLayout(false);
            this.grpResult.PerformLayout();
            this.grpConfiguration.ResumeLayout(false);
            this.grpConfiguration.PerformLayout();
            this.grpMethod.ResumeLayout(false);
            this.grpMethod.PerformLayout();
            this.panelData.ResumeLayout(false);
            this.panelData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvIntegralData)).EndInit();
            this.panelFunction.ResumeLayout(false);
            this.panelFunction.PerformLayout();
            this.grpInputType.ResumeLayout(false);
            this.grpInputType.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.RichTextBox rtbDerivativeResult;
        private System.Windows.Forms.DataGridView dataXYDerivative;
        private System.Windows.Forms.DataGridViewTextBoxColumn colsXDerivative;
        private System.Windows.Forms.DataGridViewTextBoxColumn colsYDerivative;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnDerivative;
        private System.Windows.Forms.Button btnOpenExcelDerivative;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtBoxXDerivative;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxDerivative;
        private System.Windows.Forms.TextBox txtBoxPrecisionDerivative;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox grpInputType;
        private System.Windows.Forms.RadioButton rdoData;
        private System.Windows.Forms.RadioButton rdoFunction;
        private System.Windows.Forms.Panel panelFunction;
        private System.Windows.Forms.TextBox txtUpperBound;
        private System.Windows.Forms.Label lblUpperBound;
        private System.Windows.Forms.TextBox txtLowerBound;
        private System.Windows.Forms.Label lblLowerBound;
        private System.Windows.Forms.Label lblFunctionHint;
        private System.Windows.Forms.TextBox txtFunction;
        private System.Windows.Forms.Label lblFunction;
        private System.Windows.Forms.Panel panelData;
        private System.Windows.Forms.TextBox txtDataEpsilon;
        private System.Windows.Forms.Label lblDataEpsilon;
        private System.Windows.Forms.TextBox txtDataUpperBound;
        private System.Windows.Forms.Label lblDataUpperBound;
        private System.Windows.Forms.TextBox txtDataLowerBound;
        private System.Windows.Forms.Label lblDataLowerBound;
        private System.Windows.Forms.DataGridView dgvIntegralData;
        private System.Windows.Forms.Button btnClearDataIntegral;
        private System.Windows.Forms.Button btnOpenExcelIntegral;
        private System.Windows.Forms.GroupBox grpMethod;
        private System.Windows.Forms.ComboBox cmbMethod;
        private System.Windows.Forms.Label lblMethod;
        private System.Windows.Forms.GroupBox grpConfiguration;
        private System.Windows.Forms.TextBox txtEpsilon;
        private System.Windows.Forms.Label lblEpsilon;
        private System.Windows.Forms.TextBox txtN;
        private System.Windows.Forms.Label lblN;
        private System.Windows.Forms.RadioButton rdoCalculateN;
        private System.Windows.Forms.RadioButton rdoFixedN;
        private System.Windows.Forms.Button btnCalculateIntegral;
        private System.Windows.Forms.GroupBox grpResult;
        private System.Windows.Forms.Label lblIntegralResult;
        private System.Windows.Forms.RichTextBox rtbIntegralResult;
        private System.Windows.Forms.DataGridViewTextBoxColumn colsXIntegral;
        private System.Windows.Forms.DataGridViewTextBoxColumn colsYIntegral;
        private System.Windows.Forms.TextBox txtBoxFunction;
        private System.Windows.Forms.Label label2;
    }
}