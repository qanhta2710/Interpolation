namespace Interpolation
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.dataGridViewLagrange = new System.Windows.Forms.DataGridView();
            this.lblResult = new System.Windows.Forms.Label();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.btnSolveLagrange = new System.Windows.Forms.Button();
            this.txtBoxPrecisionLagrange = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dataXYLagrange = new System.Windows.Forms.DataGridView();
            this.colsXLagrange = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colsYLagrange = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.txtBoxPrecision = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtBoxn = new System.Windows.Forms.TextBox();
            this.txtBoxb = new System.Windows.Forms.TextBox();
            this.txtBoxa = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.btnSolveChebyshev = new System.Windows.Forms.Button();
            this.dataChebyshev = new System.Windows.Forms.DataGridView();
            this.a = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.b = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.n = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.chebyshevPoints = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.dataGridViewNewton = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.dataXYNewton = new System.Windows.Forms.DataGridView();
            this.colsXNewton = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colsYNewton = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.lblResultNewton = new System.Windows.Forms.Label();
            this.btnSolveNewton = new System.Windows.Forms.Button();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.txtBoxPrecisionNewton = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxNewton = new System.Windows.Forms.ComboBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.dataGridViewCoeffsP = new System.Windows.Forms.DataGridView();
            this.coeffsP = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel2 = new System.Windows.Forms.Panel();
            this.txtBoxPrecisionEval = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.txtBoxC = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.richTextBoxResult = new System.Windows.Forms.RichTextBox();
            this.tabPage2.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewLagrange)).BeginInit();
            this.flowLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataXYLagrange)).BeginInit();
            this.tabPage1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataChebyshev)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewNewton)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataXYNewton)).BeginInit();
            this.tableLayoutPanel5.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCoeffsP)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.tableLayoutPanel4);
            this.tabPage2.Controls.Add(this.flowLayoutPanel2);
            this.tabPage2.Controls.Add(this.dataXYLagrange);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1086, 612);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Nội suy Lagrange";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 1;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Controls.Add(this.dataGridViewLagrange, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.lblResult, 0, 1);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(261, 3);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 2;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 91.22486F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.775137F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(822, 547);
            this.tableLayoutPanel4.TabIndex = 4;
            // 
            // dataGridViewLagrange
            // 
            this.dataGridViewLagrange.AllowUserToAddRows = false;
            this.dataGridViewLagrange.AllowUserToDeleteRows = false;
            this.dataGridViewLagrange.AllowUserToResizeColumns = false;
            this.dataGridViewLagrange.AllowUserToResizeRows = false;
            this.dataGridViewLagrange.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewLagrange.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewLagrange.Location = new System.Drawing.Point(3, 3);
            this.dataGridViewLagrange.Name = "dataGridViewLagrange";
            this.dataGridViewLagrange.ReadOnly = true;
            this.dataGridViewLagrange.RowHeadersWidth = 51;
            this.dataGridViewLagrange.RowTemplate.Height = 24;
            this.dataGridViewLagrange.Size = new System.Drawing.Size(816, 493);
            this.dataGridViewLagrange.TabIndex = 6;
            // 
            // lblResult
            // 
            this.lblResult.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblResult.AutoSize = true;
            this.lblResult.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblResult.Location = new System.Drawing.Point(373, 511);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(76, 23);
            this.lblResult.TabIndex = 7;
            this.lblResult.Text = "label2";
            this.lblResult.Visible = false;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.tableLayoutPanel2);
            this.flowLayoutPanel2.Controls.Add(this.label1);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(261, 550);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(822, 59);
            this.flowLayoutPanel2.TabIndex = 3;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.btnSolveLagrange, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.txtBoxPrecisionLagrange, 0, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(308, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 49F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(511, 49);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // btnSolveLagrange
            // 
            this.btnSolveLagrange.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSolveLagrange.AutoSize = true;
            this.btnSolveLagrange.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSolveLagrange.Location = new System.Drawing.Point(223, 8);
            this.btnSolveLagrange.Name = "btnSolveLagrange";
            this.btnSolveLagrange.Size = new System.Drawing.Size(170, 33);
            this.btnSolveLagrange.TabIndex = 0;
            this.btnSolveLagrange.Text = "Tìm đa thức nội suy";
            this.btnSolveLagrange.UseVisualStyleBackColor = true;
            this.btnSolveLagrange.Click += new System.EventHandler(this.btnSolveLagrange_Click);
            // 
            // txtBoxPrecisionLagrange
            // 
            this.txtBoxPrecisionLagrange.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtBoxPrecisionLagrange.Location = new System.Drawing.Point(3, 13);
            this.txtBoxPrecisionLagrange.Name = "txtBoxPrecisionLagrange";
            this.txtBoxPrecisionLagrange.Size = new System.Drawing.Size(100, 22);
            this.txtBoxPrecisionLagrange.TabIndex = 1;
            this.txtBoxPrecisionLagrange.Text = "15";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(94, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(208, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Số chữ số hiển thị phần thập phân";
            // 
            // dataXYLagrange
            // 
            this.dataXYLagrange.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataXYLagrange.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataXYLagrange.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colsXLagrange,
            this.colsYLagrange});
            this.dataXYLagrange.Dock = System.Windows.Forms.DockStyle.Left;
            this.dataXYLagrange.Location = new System.Drawing.Point(3, 3);
            this.dataXYLagrange.Name = "dataXYLagrange";
            this.dataXYLagrange.RowHeadersWidth = 51;
            this.dataXYLagrange.RowTemplate.Height = 24;
            this.dataXYLagrange.Size = new System.Drawing.Size(258, 606);
            this.dataXYLagrange.TabIndex = 2;
            // 
            // colsXLagrange
            // 
            this.colsXLagrange.HeaderText = "X";
            this.colsXLagrange.MinimumWidth = 6;
            this.colsXLagrange.Name = "colsXLagrange";
            // 
            // colsYLagrange
            // 
            this.colsYLagrange.HeaderText = "Y";
            this.colsYLagrange.MinimumWidth = 6;
            this.colsYLagrange.Name = "colsYLagrange";
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.flowLayoutPanel1);
            this.tabPage1.Controls.Add(this.dataChebyshev);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1086, 612);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Mốc nội suy Chebyshev";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.tableLayoutPanel1);
            this.flowLayoutPanel1.Controls.Add(this.tableLayoutPanel3);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1080, 240);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70.62937F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 29.37063F));
            this.tableLayoutPanel1.Controls.Add(this.txtBoxPrecision, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label7, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.txtBoxn, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.txtBoxb, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.txtBoxa, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label6, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 45.27027F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 65F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 65F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(462, 231);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // txtBoxPrecision
            // 
            this.txtBoxPrecision.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtBoxPrecision.Location = new System.Drawing.Point(344, 187);
            this.txtBoxPrecision.Name = "txtBoxPrecision";
            this.txtBoxPrecision.Size = new System.Drawing.Size(100, 22);
            this.txtBoxPrecision.TabIndex = 0;
            this.txtBoxPrecision.Text = "15";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(126, 12);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(73, 25);
            this.label4.TabIndex = 0;
            this.label4.Text = "Nhập a:";
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(20, 186);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(285, 25);
            this.label7.TabIndex = 1;
            this.label7.Text = "Số chữ số hiển thị phần thập phân";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(125, 63);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(75, 25);
            this.label5.TabIndex = 1;
            this.label5.Text = "Nhập b:";
            // 
            // txtBoxn
            // 
            this.txtBoxn.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtBoxn.Location = new System.Drawing.Point(344, 122);
            this.txtBoxn.Name = "txtBoxn";
            this.txtBoxn.Size = new System.Drawing.Size(100, 22);
            this.txtBoxn.TabIndex = 4;
            // 
            // txtBoxb
            // 
            this.txtBoxb.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtBoxb.Location = new System.Drawing.Point(344, 64);
            this.txtBoxb.Name = "txtBoxb";
            this.txtBoxb.Size = new System.Drawing.Size(100, 22);
            this.txtBoxb.TabIndex = 3;
            // 
            // txtBoxa
            // 
            this.txtBoxa.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtBoxa.Location = new System.Drawing.Point(344, 14);
            this.txtBoxa.Name = "txtBoxa";
            this.txtBoxa.Size = new System.Drawing.Size(100, 22);
            this.txtBoxa.TabIndex = 2;
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(46, 121);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(234, 25);
            this.label6.TabIndex = 2;
            this.label6.Text = "Nhập bậc đa thức nội suy n:";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Controls.Add(this.btnSolveChebyshev, 0, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(471, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(200, 231);
            this.tableLayoutPanel3.TabIndex = 3;
            // 
            // btnSolveChebyshev
            // 
            this.btnSolveChebyshev.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSolveChebyshev.Location = new System.Drawing.Point(26, 77);
            this.btnSolveChebyshev.Name = "btnSolveChebyshev";
            this.btnSolveChebyshev.Size = new System.Drawing.Size(147, 56);
            this.btnSolveChebyshev.TabIndex = 0;
            this.btnSolveChebyshev.Text = "Tìm mốc nội suy";
            this.btnSolveChebyshev.UseVisualStyleBackColor = true;
            this.btnSolveChebyshev.Click += new System.EventHandler(this.btnSolveChebyshev_Click);
            // 
            // dataChebyshev
            // 
            this.dataChebyshev.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataChebyshev.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.a,
            this.b,
            this.n,
            this.chebyshevPoints});
            this.dataChebyshev.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dataChebyshev.Location = new System.Drawing.Point(3, 243);
            this.dataChebyshev.Name = "dataChebyshev";
            this.dataChebyshev.RowHeadersWidth = 51;
            this.dataChebyshev.RowTemplate.Height = 24;
            this.dataChebyshev.Size = new System.Drawing.Size(1080, 366);
            this.dataChebyshev.TabIndex = 0;
            // 
            // a
            // 
            this.a.HeaderText = "a";
            this.a.MinimumWidth = 6;
            this.a.Name = "a";
            this.a.Width = 125;
            // 
            // b
            // 
            this.b.HeaderText = "b";
            this.b.MinimumWidth = 6;
            this.b.Name = "b";
            this.b.Width = 125;
            // 
            // n
            // 
            this.n.HeaderText = "n";
            this.n.MinimumWidth = 6;
            this.n.Name = "n";
            this.n.Width = 125;
            // 
            // chebyshevPoints
            // 
            this.chebyshevPoints.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.chebyshevPoints.HeaderText = "Mốc nội suy";
            this.chebyshevPoints.MinimumWidth = 6;
            this.chebyshevPoints.Name = "chebyshevPoints";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1094, 641);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.dataGridViewNewton);
            this.tabPage3.Controls.Add(this.panel1);
            this.tabPage3.Controls.Add(this.tableLayoutPanel5);
            this.tabPage3.Location = new System.Drawing.Point(4, 25);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(1086, 612);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Nội suy Newton mốc bất kì";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // dataGridViewNewton
            // 
            this.dataGridViewNewton.AllowUserToAddRows = false;
            this.dataGridViewNewton.AllowUserToDeleteRows = false;
            this.dataGridViewNewton.AllowUserToResizeColumns = false;
            this.dataGridViewNewton.AllowUserToResizeRows = false;
            this.dataGridViewNewton.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewNewton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewNewton.Location = new System.Drawing.Point(251, 3);
            this.dataGridViewNewton.Name = "dataGridViewNewton";
            this.dataGridViewNewton.ReadOnly = true;
            this.dataGridViewNewton.RowHeadersWidth = 51;
            this.dataGridViewNewton.RowTemplate.Height = 24;
            this.dataGridViewNewton.Size = new System.Drawing.Size(832, 506);
            this.dataGridViewNewton.TabIndex = 4;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.dataXYNewton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(248, 506);
            this.panel1.TabIndex = 3;
            // 
            // dataXYNewton
            // 
            this.dataXYNewton.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataXYNewton.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colsXNewton,
            this.colsYNewton});
            this.dataXYNewton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataXYNewton.Location = new System.Drawing.Point(0, 0);
            this.dataXYNewton.Name = "dataXYNewton";
            this.dataXYNewton.RowHeadersWidth = 51;
            this.dataXYNewton.RowTemplate.Height = 24;
            this.dataXYNewton.Size = new System.Drawing.Size(248, 506);
            this.dataXYNewton.TabIndex = 1;
            // 
            // colsXNewton
            // 
            this.colsXNewton.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colsXNewton.HeaderText = "X";
            this.colsXNewton.MinimumWidth = 6;
            this.colsXNewton.Name = "colsXNewton";
            // 
            // colsYNewton
            // 
            this.colsYNewton.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colsYNewton.HeaderText = "Y";
            this.colsYNewton.MinimumWidth = 6;
            this.colsYNewton.Name = "colsYNewton";
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 2;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 27.5F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 72.5F));
            this.tableLayoutPanel5.Controls.Add(this.lblResultNewton, 1, 0);
            this.tableLayoutPanel5.Controls.Add(this.btnSolveNewton, 1, 1);
            this.tableLayoutPanel5.Controls.Add(this.flowLayoutPanel3, 0, 1);
            this.tableLayoutPanel5.Controls.Add(this.comboBoxNewton, 0, 0);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(3, 509);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 2;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(1080, 100);
            this.tableLayoutPanel5.TabIndex = 2;
            // 
            // lblResultNewton
            // 
            this.lblResultNewton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblResultNewton.AutoSize = true;
            this.lblResultNewton.Font = new System.Drawing.Font("Consolas", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblResultNewton.Location = new System.Drawing.Point(653, 14);
            this.lblResultNewton.Name = "lblResultNewton";
            this.lblResultNewton.Size = new System.Drawing.Size(70, 22);
            this.lblResultNewton.TabIndex = 0;
            this.lblResultNewton.Text = "label2";
            this.lblResultNewton.Visible = false;
            // 
            // btnSolveNewton
            // 
            this.btnSolveNewton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSolveNewton.AutoSize = true;
            this.btnSolveNewton.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSolveNewton.Location = new System.Drawing.Point(599, 57);
            this.btnSolveNewton.Name = "btnSolveNewton";
            this.btnSolveNewton.Size = new System.Drawing.Size(178, 35);
            this.btnSolveNewton.TabIndex = 1;
            this.btnSolveNewton.Text = "Tìm đa thức nội suy";
            this.btnSolveNewton.UseVisualStyleBackColor = true;
            this.btnSolveNewton.Click += new System.EventHandler(this.btnSolveNewton_Click);
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.Controls.Add(this.txtBoxPrecisionNewton);
            this.flowLayoutPanel3.Controls.Add(this.label3);
            this.flowLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel3.Location = new System.Drawing.Point(3, 53);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(291, 44);
            this.flowLayoutPanel3.TabIndex = 2;
            // 
            // txtBoxPrecisionNewton
            // 
            this.txtBoxPrecisionNewton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtBoxPrecisionNewton.Location = new System.Drawing.Point(3, 3);
            this.txtBoxPrecisionNewton.Name = "txtBoxPrecisionNewton";
            this.txtBoxPrecisionNewton.Size = new System.Drawing.Size(100, 22);
            this.txtBoxPrecisionNewton.TabIndex = 0;
            this.txtBoxPrecisionNewton.Text = "15";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(109, 6);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(161, 16);
            this.label3.TabIndex = 1;
            this.label3.Text = "Số chữ số phần thập phân";
            // 
            // comboBoxNewton
            // 
            this.comboBoxNewton.FormattingEnabled = true;
            this.comboBoxNewton.Items.AddRange(new object[] {
            "Mốc nội suy bất kì",
            "Mốc nội suy tăng dần",
            "Mốc nội suy giảm dần"});
            this.comboBoxNewton.Location = new System.Drawing.Point(3, 3);
            this.comboBoxNewton.Name = "comboBoxNewton";
            this.comboBoxNewton.Size = new System.Drawing.Size(164, 24);
            this.comboBoxNewton.TabIndex = 3;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.richTextBoxResult);
            this.tabPage4.Controls.Add(this.panel2);
            this.tabPage4.Controls.Add(this.dataGridViewCoeffsP);
            this.tabPage4.Location = new System.Drawing.Point(4, 25);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(1086, 612);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Tính giá trị";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // dataGridViewCoeffsP
            // 
            this.dataGridViewCoeffsP.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewCoeffsP.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.coeffsP});
            this.dataGridViewCoeffsP.Dock = System.Windows.Forms.DockStyle.Left;
            this.dataGridViewCoeffsP.Location = new System.Drawing.Point(3, 3);
            this.dataGridViewCoeffsP.Name = "dataGridViewCoeffsP";
            this.dataGridViewCoeffsP.RowHeadersWidth = 51;
            this.dataGridViewCoeffsP.RowTemplate.Height = 24;
            this.dataGridViewCoeffsP.Size = new System.Drawing.Size(166, 606);
            this.dataGridViewCoeffsP.TabIndex = 0;
            // 
            // coeffsP
            // 
            this.coeffsP.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.coeffsP.HeaderText = "Hệ số";
            this.coeffsP.MinimumWidth = 6;
            this.coeffsP.Name = "coeffsP";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label8);
            this.panel2.Controls.Add(this.txtBoxC);
            this.panel2.Controls.Add(this.button1);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.txtBoxPrecisionEval);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(169, 489);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(914, 120);
            this.panel2.TabIndex = 1;
            // 
            // txtBoxPrecisionEval
            // 
            this.txtBoxPrecisionEval.Location = new System.Drawing.Point(6, 73);
            this.txtBoxPrecisionEval.Name = "txtBoxPrecisionEval";
            this.txtBoxPrecisionEval.Size = new System.Drawing.Size(182, 22);
            this.txtBoxPrecisionEval.TabIndex = 0;
            this.txtBoxPrecisionEval.Text = "15";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(6, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(180, 18);
            this.label2.TabIndex = 1;
            this.label2.Text = "Số chữ số phần thập phân";
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(801, 42);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(108, 30);
            this.button1.TabIndex = 2;
            this.button1.Text = "Tính giá trị";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtBoxC
            // 
            this.txtBoxC.Location = new System.Drawing.Point(281, 73);
            this.txtBoxC.Name = "txtBoxC";
            this.txtBoxC.Size = new System.Drawing.Size(102, 22);
            this.txtBoxC.TabIndex = 3;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(281, 51);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(102, 18);
            this.label8.TabIndex = 4;
            this.label8.Text = "Giá trị cần tính";
            // 
            // richTextBoxResult
            // 
            this.richTextBoxResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxResult.Location = new System.Drawing.Point(169, 3);
            this.richTextBoxResult.Name = "richTextBoxResult";
            this.richTextBoxResult.ReadOnly = true;
            this.richTextBoxResult.Size = new System.Drawing.Size(914, 486);
            this.richTextBoxResult.TabIndex = 2;
            this.richTextBoxResult.Text = "";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1094, 641);
            this.Controls.Add(this.tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Nội suy";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabPage2.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewLagrange)).EndInit();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataXYLagrange)).EndInit();
            this.tabPage1.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataChebyshev)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewNewton)).EndInit();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataXYNewton)).EndInit();
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            this.flowLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCoeffsP)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.DataGridView dataGridViewLagrange;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button btnSolveLagrange;
        private System.Windows.Forms.TextBox txtBoxPrecisionLagrange;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dataXYLagrange;
        private System.Windows.Forms.DataGridViewTextBoxColumn colsXLagrange;
        private System.Windows.Forms.DataGridViewTextBoxColumn colsYLagrange;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox txtBoxPrecision;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtBoxn;
        private System.Windows.Forms.TextBox txtBoxb;
        private System.Windows.Forms.TextBox txtBoxa;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Button btnSolveChebyshev;
        private System.Windows.Forms.DataGridView dataChebyshev;
        private System.Windows.Forms.DataGridViewTextBoxColumn a;
        private System.Windows.Forms.DataGridViewTextBoxColumn b;
        private System.Windows.Forms.DataGridViewTextBoxColumn n;
        private System.Windows.Forms.DataGridViewTextBoxColumn chebyshevPoints;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.Label lblResult;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.DataGridView dataXYNewton;
        private System.Windows.Forms.DataGridViewTextBoxColumn colsXNewton;
        private System.Windows.Forms.DataGridViewTextBoxColumn colsYNewton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblResultNewton;
        private System.Windows.Forms.Button btnSolveNewton;
        private System.Windows.Forms.DataGridView dataGridViewNewton;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.TextBox txtBoxPrecisionNewton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBoxNewton;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataGridView dataGridViewCoeffsP;
        private System.Windows.Forms.DataGridViewTextBoxColumn coeffsP;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtBoxPrecisionEval;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtBoxC;
        private System.Windows.Forms.RichTextBox richTextBoxResult;
    }
}

