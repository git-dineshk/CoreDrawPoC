using System;
using System.Windows.Controls;
using corel = Corel.Interop.VGCore;
using System.Data.SqlClient;
using System.Data;

namespace DockerTemplateCS2
{

    public partial class DockerUI : UserControl
    {
        private corel.Application corelApp;
        public DockerUI(corel.Application app)
        {
            this.corelApp = app;
            InitializeComponent();
            btn_drawCicle.Click += (s, e) => { app.ActiveDocument.ActiveLayer.CreateEllipse(0, 0, 2, 2); };
            btn_drawSquad.Click += (s, e) => { app.ActiveDocument.ActiveLayer.CreateRectangle(0, 0, 3, 2); };
            btn_changeColor.Click += (s, e) => { ChangeColor(app); };
            btn_ReadText.Click += (s, e) => { ReadText(app); };
            btn_LoadData.Click += (s, e) => { LoadData(app); };
            btn_linkText.Click += (s, e) => { LinkText(app); };
        }

        private void LinkText(corel.Application app)
        {
            try
            {
                corel.ShapeRange sr = app.ActiveSelectionRange;
                for (int i = 1; i <= sr.Count; i++)
                {
                    if (sr[i] != null && sr[i].Type != corel.cdrShapeType.cdrTextShape)
                    {
                        txtBox_Info.Text = "Add Text";
                        corel.Layer s_l = sr[i].Layer;
                        corel.Shape s1 = s_l.CreateArtisticText(5, 6, "Text_123");
                        s1.Fill.UniformColor.CMYKAssign(0, 0, 0, 100);
                        s1.Outline.SetNoOutline();
                        corel.Shape s2 = s_l.CreateConnectorEx(corel.cdrConnectorType.cdrConnectorStraight, sr[i].SnapPoints.Object(corel.cdrObjectSnapPointType.cdrObjectPointRight), app.ActiveDocument.CreateFreeSnapPoint(6, 6));
                        s2.Connector.EndPoint = s1.SnapPoints.Object(corel.cdrObjectSnapPointType.cdrObjectPointTop);
                    }
                }
                //corel.Shapes sh = app.ActiveDocument.ActiveLayer.Shapes;
                //for (int i = 1; i <= sh.Count; i++)
                //{
                //    corel.Shape s = sh[i];
                //    if (s.Selected && s.Type != corel.cdrShapeType.cdrTextShape)
                //    {
                //        corel.Shape s1 = app.ActiveLayer.CreateArtisticText(5, 6, "Text_123");
                //        s1.Fill.UniformColor.CMYKAssign(0, 0, 0, 100);
                //        s1.Outline.SetNoOutline();
                //        corel.Shape s2 = app.ActiveLayer.CreateConnectorEx(corel.cdrConnectorType.cdrConnectorStraight, s.SnapPoints.Object(corel.cdrObjectSnapPointType.cdrObjectPointRight), app.ActiveDocument.CreateFreeSnapPoint(6, 6));
                //        s2.Connector.EndPoint = s1.SnapPoints.Object(corel.cdrObjectSnapPointType.cdrObjectPointTop);
                //    }
                //}
            }
            catch (Exception ex)
            {
                txtBox_Info.Text = ex.Message;
            }
            finally
            {
            }
        }

        private void LoadData(corel.Application app)
        {
            SqlConnection sqlConn = null;
            try
            {
                // Connecting the SQL Server
                sqlConn = new SqlConnection("Data Source=(local)\\sqlexpress;Initial Catalog=Sample;Integrated Security=True");
                // Calling the Stored Procedure
                SqlCommand cmd = new SqlCommand("ItemInfo", sqlConn);
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT [ItemId], [ItemName], [ItemDesc] FROM [Sample].[dbo].[ItemInfo]";
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                sqlConn.Open();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    // Binding the Grid with the Itemsource property
                    itemGrid.ItemsSource = ds.Tables[0].DefaultView;
                }
            }
            catch (Exception ex)
            {
                txtBox_Info.Text = ex.Message;
            }
            finally
            {
                if (sqlConn != null && sqlConn.State == ConnectionState.Open)
                {
                    sqlConn.Close();
                }
            }
        }

        private void ReadText(corel.Application app)
        {
            try
            {
                corel.Documents docs = app.Documents;
                foreach (corel.Document d in docs)
                {
                    txtBox_Info.Text += d.Name + "\r\n";
                    corel.Pages pgs = d.Pages;
                    foreach (corel.Page p in pgs)
                    {
                        txtBox_Info.Text += p.Name + "\r\n";
                        corel.Layers lyrs = p.Layers;
                        foreach (corel.Layer l in lyrs)
                        {
                            Corel.Interop.VGCore.ShapeRange sr = l.Shapes.FindShapes();
                            txtBox_Info.Text += l.Name + " *** Number of objects : " + sr.Count + "\r\n";
                            for (int i = 1; i <= sr.Count; i++)
                            {
                                if (sr[i] != null && sr[i].Type == corel.cdrShapeType.cdrTextShape)
                                {
                                    txtBox_Info.Text += sr[i].Text.Story.Text + "\r\n";
                                }
                            }
                        }
                        txtBox_Info.Text += "*****-----*****\r\n";
                    }
                }
            }
            catch (Exception ex)
            {
                txtBox_Info.Text = ex.Message;
            }
        }

        private void CurveArea(Corel.Interop.VGCore.Application app)
        {
            global::System.Windows.MessageBox.Show(string.Format("Area: {0}", app.ActiveShape.Curve.Area));
        }

        public void DrawArrow(Corel.Interop.VGCore.Application app)
        {
            Corel.Interop.VGCore.Shape line = app.ActiveDocument.ActiveLayer.CreateLineSegment(0, 0, 1.5, 0);
            line.Outline.EndArrow = app.ArrowHeads[2];
        }
        public void ChangeColor(Corel.Interop.VGCore.Application app)
        {
            try
            {
                corel.Shapes sh = app.ActiveDocument.ActiveLayer.Shapes;
                Random random = new Random();
                for (int i = 1; i <= sh.Count; i++)
                {
                    corel.Shape s = sh[i];
                    if (s.Selected)
                    {
                        s.Fill.UniformColor.CMYAssign(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
                    }
                }
            }
            catch (Exception ex)
            {
                txtBox_Info.Text = ex.Message;
            }
        }

    }
}
