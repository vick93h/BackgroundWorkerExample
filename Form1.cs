using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
namespace BackgroundWorkerExample
{
    
    public partial class Form1 : Form
    {
        BackgroundWorker worker;
  
        public Form1()
        {
           InitializeComponent();
            worker=new BackgroundWorker();
            worker.DoWork += backgroundWorker1_DoWork;
            worker.ProgressChanged += backgroundWorker1_ProgressChanged;
            worker.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
            worker.WorkerReportsProgress=true;
            progressBar1.Value = 0;
            progressBar1.Step = 10;
            progressBar1.Maximum = 100;
            progressBar1.Visible = false;
            label1.Visible = false;
            
        }
   


        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: questa riga di codice carica i dati nella tabella 'adventureWorks2019DataSet.Customer'. È possibile spostarla o rimuoverla se necessario.
            this.customerTableAdapter.Fill(this.adventureWorks2019DataSet.Customer);
            

        }


        private void button1_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "Excel Documents (*.xlsx)|*.xlsx";
            DateTime date = DateTime.Now;
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                progressBar1.Visible = true;
                label1.Visible = true;
                worker.RunWorkerAsync();
            }
          
        }
 

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {      worker.ReportProgress(10);
                    using (ExcelPackage pck = new ExcelPackage())
                    {
                        worker.ReportProgress(50);
                        foreach (DataTable dataTable in adventureWorks2019DataSet.Tables)
                        {
                            ExcelWorksheet ws = pck.Workbook.Worksheets.Add(dataTable.TableName);
                            ws.Cells["A1"].LoadFromDataTable(dataTable, true);
                            ws.Cells.AutoFitColumns();

                        }
                        worker.ReportProgress(80);
                        pck.Save();
                    }
                    worker.ReportProgress(100);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Eccezione: " + ex.Message + " Stacktrace: " + ex.StackTrace);
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            label1.Text = e.ProgressPercentage.ToString() + "%";
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar1.Visible = false;
            label1.Visible = false;
            MessageBox.Show("Finished!");
        }
    }
}
