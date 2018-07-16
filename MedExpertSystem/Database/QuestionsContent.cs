using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MedExpertSystem.Models;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace MedExpertSystem.Database
{
    public class QuestionsContent
    {
        public List<QuestionsDataDefinitionModel> QuestionsBase { get; set; }

        public string symptom;
        public int idSympt;
        public int Numb;

        public QuestionsContent()
        {
            string strConnString = ConfigurationManager.ConnectionStrings["entityFramework"].ConnectionString;
            if (QuestionsBase ==null) QuestionsBase = new List<QuestionsDataDefinitionModel>();
            using (SqlConnection con = new SqlConnection(strConnString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "Get_HA";
                    cmd.CommandType = CommandType.StoredProcedure;
                    try
                    {
                        if (con.State != ConnectionState.Open)
                            con.Open();
                        cmd.Connection = con;
                       cmd.Parameters.Add("@ID", SqlDbType.Int).Direction = ParameterDirection.Output;
                       cmd.Parameters.Add("@Numb", SqlDbType.Int).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("@Symptom", SqlDbType.NVarChar, 150).Direction = ParameterDirection.Output;
                        cmd.ExecuteNonQuery();
                        idSympt = Convert.ToInt32(cmd.Parameters["@ID"].Value);
                        Numb = Convert.ToInt32(cmd.Parameters["@Numb"].Value);
                        symptom =cmd.Parameters["@Symptom"].Value.ToString();

                        con.Close();
                    }
                    catch (Exception expt)
                    {
                        throw new Exception(expt.Message);
                    }
                }
             
            }

                QuestionsBase.Add(new QuestionsDataDefinitionModel
                {
                    Index = Numb-1,
                    QuestionAnswerOptionOne = idSympt.ToString() +"- " + symptom //"У Вас наблюдается " + symptom + " ?"

                });
          
            
        }
    }
}    

