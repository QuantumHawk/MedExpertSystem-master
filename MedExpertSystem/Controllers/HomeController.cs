using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MedExpertSystem.Database;
using MedExpertSystem.Models;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace MedExpertSystem.Controllers
{
    public class HomeController : BaseController
    {
      public  string strConnString = ConfigurationManager.ConnectionStrings["entityFramework"].ConnectionString;
      public int idSymp;
      public DataSet dsResult;
      public DataTable tblResult;
      public DataTable tblExpert;
      public DataTable tblFinal;
      public bool stop = false;
      public string data;

        public HomeController()
        {
            if (Globals.Globals.Index != 0)
            {

            }
            else
            {
                Random r = new Random();
                Globals.Globals.Index = r.Next(1, 12);
            }
        }

        [HttpGet]
        public ActionResult RegistrationForm() // Форма регистрации пользователя
        {
            return View("RegistrationForm");
        }



        [HttpPost]
        public ActionResult RegistrationForm(string Name, string Surname) // Форма регистрации пользователя
        {
            Session["currentUserName"] = Name;
            Session["currentUserSurname"] = Surname;
            UserAnswer.Name = Name;
            UserAnswer.Surname = Surname;

            if (Request.Form["firstTest"] != null)
            {
                ClearTibetanTestRes();
                return RedirectToAction("TibetanQuestionForm", new { index = 0 });
          
            }
            else           
            ClearTestRes();
            return RedirectToAction("QuestionForm", new { index = 0 });
           
           
        }

        public void ClearTibetanTestRes()  // Очистка результатов прохождения теста по определению типа конституции
        {
            UserAnswer.FAnsw = 0;
            UserAnswer.SAnsw = 0;
            UserAnswer.TAnsw = 0;
            Session["TAQuestionsBase"] = null; 

        }
        public void ClearTestRes() // Очистка результатов прохождения Основного теста 
        {
            using (SqlConnection con = new SqlConnection(strConnString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "ClearTest";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    try
                    {
                        if (con.State != ConnectionState.Open)
                            con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                    catch (Exception expt)
                    {
                        throw new Exception(expt.Message);
                    }
                }

            }
        }


        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ExpertPageAuthorization() //Авторизация эксперта
        {
            return View();
        }

    
        [HttpPost]
        public ActionResult ExpertPageAuthorization(Expert expert) //Авторизация эксперта
        {
            Database.Database.Experts.Add(new Expert { Name = "Admin", Password = "123" });
            var find = Database.Database.Experts.FirstOrDefault(p => p.Name == expert.Name && p.Password == expert.Password);
            if (find != null)
            {
                Session["CurrentUser"] = find;
                return RedirectToAction("ExpertPoll");
            }
            else
            {
                return RedirectToAction("Index");
            }
            
        }

        public void ExpertTable()
        {

            tblExpert = new DataTable("DataList");
            using (SqlConnection con = new SqlConnection(strConnString))
            {
                if (con.State != ConnectionState.Open)
                    con.Open();
                using (SqlCommand cmd = new SqlCommand("Get_ExpertTable", con))
                {
                    var reader = cmd.ExecuteReader();
                    var schemaTable = reader.GetSchemaTable();
                    tblExpert = GetTable(schemaTable);
                    while (reader.Read())
                    {
                        var values = new object[reader.FieldCount];
                        reader.GetValues(values);
                        tblExpert.Rows.Add(values);

                    }
                    con.Close();
                }
            }
            Session["nameTable"] = "BK_ApriorProb";
        }
        public void ExpertTableWithFilter(string filter, string nameTable)
        {
            //TODO: проверить, что фильтр задан правильно: ">=0.5,<= > < порядок символов должен быть правильный"
            tblExpert = new DataTable("DataList");
            using (SqlConnection con = new SqlConnection(strConnString))
            {
                if (con.State != ConnectionState.Open)
                    con.Open();
                using (SqlCommand cmd = new SqlCommand("GetTableByFilter", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    try
                    {

                        cmd.Parameters.AddWithValue("@filter", filter);
                        cmd.Parameters.AddWithValue("@nameTable", nameTable);
                        var reader = cmd.ExecuteReader();
                        var schemaTable = reader.GetSchemaTable();
                        tblExpert = GetTable(schemaTable);
                        while (reader.Read())
                        {
                            var values = new object[reader.FieldCount];
                            reader.GetValues(values);
                            tblExpert.Rows.Add(values);

                        }
                    }
                    catch (Exception expt)
                    {
                        throw new Exception(expt.Message);
                    }
                }
                con.Close();
            }
            //Session["nameTable"] = nameTable;
            Session["nameTable"] = "FilteredTable";
        }

        public void ExpertTableDQ(string diagnos, string question, string nameTable)
        {

            tblExpert = new DataTable("DataList");
            using (SqlConnection con = new SqlConnection(strConnString))
            {
                if (con.State != ConnectionState.Open)
                    con.Open();
                using (SqlCommand cmd = new SqlCommand("GetTableForEdit", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    try
                    {
                       
                        cmd.Parameters.AddWithValue("@questions", question);
                        cmd.Parameters.AddWithValue("@diagnosis", diagnos);
                        cmd.Parameters.AddWithValue("@nameTable", nameTable);

                        var reader = cmd.ExecuteReader();
                        var schemaTable = reader.GetSchemaTable();
                        tblExpert = GetTable(schemaTable);
                        while (reader.Read())
                        {
                            var values = new object[reader.FieldCount];
                            reader.GetValues(values);
                            tblExpert.Rows.Add(values);

                        }
                    }
                    catch (Exception expt)
                    {
                        throw new Exception(expt.Message);
                    }
                }
                con.Close();
            }
            Session["nameTable"] = nameTable;
        }

        [HttpGet]
        public ActionResult ExpertPoll()
        {
                Get_Q_D(true);
                ExpertTable();
            return View(tblExpert);
        }

        public void ExpertRestore(string nameTable)
        {
            using (SqlConnection con = new SqlConnection(strConnString))
            {
                if (con.State != ConnectionState.Open)
                    con.Open();
                using (SqlCommand cmd = new SqlCommand("Restore", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    try
                    {
                        cmd.Parameters.AddWithValue("@nameTable", nameTable);
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception expt)
                    {
                        throw new Exception(expt.Message);
                    }
                }
                con.Close();
            }
        }

        [HttpPost]
        public ActionResult ExpertPoll(string nameTable, string[] diagnosis, string[] questions, string filter)
        {

            if (Request.Form["load"] != null || Request.Form["filter"] != null)
            {
                if (Request.Form["load"] != null && diagnosis != null && questions != null)
                {
                    for (int i = 0; i < diagnosis.Length; i++)
                    {
                        diagnosis[i] = diagnosis[i].Replace("+", "");
                        diagnosis[i] += " as numeric(4,3)) as " + QuestionsDataDefinitionModel.Diagnosis[i].Substring(QuestionsDataDefinitionModel.Diagnosis[i].IndexOf('-') + 1).Insert(0, "'") + "'";
                        diagnosis[i] =  diagnosis[i].Insert(0,"cast( ");
                    }
                    string diagnos = string.Join(" ,", diagnosis);
                    string question = string.Join(",", questions);

                    ExpertTableDQ(diagnos, question, nameTable);
                    Get_Q_D(true);
                    return View(tblExpert);

                }
                else

                    if (Request.Form["filter"] != null && filter !=null && filter !="") //Начать заново
                    {
                        filter = filter.Replace(",", ".");
                        ExpertTableWithFilter(filter, nameTable);
                        Get_Q_D(true);
                        return View(tblExpert);
                    }
                    if (Request.Form["restore"] != null)
                    {
                        ExpertRestore(nameTable);
                        Get_Q_D(true);
                        return View(tblExpert);
                    }
            }
            //else
            Get_Q_D(true);
            ExpertTable();
            return View(tblExpert);
        }

        private void UpdateTable(string name, int pk, string value)
        {
            string nameTable = Convert.ToString(Session["nameTable"]);
            if (nameTable != "FilteredTable")
            {
                using (SqlConnection con = new SqlConnection(strConnString))
                {
                    if (con.State != ConnectionState.Open)
                        con.Open();
                    using (SqlCommand cmd = new SqlCommand("UpdateTable", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        try
                        {

                            cmd.Parameters.AddWithValue("@name", name);
                            cmd.Parameters.AddWithValue("@pk", pk);
                            cmd.Parameters.AddWithValue("@value", value);
                            cmd.Parameters.AddWithValue("@nameTable", nameTable);
                            cmd.ExecuteNonQuery();

                        }
                        catch (Exception expt)
                        {
                            throw new Exception(expt.Message);
                        }
                    }
                    con.Close();
                }
            }
        }

        [HttpPost]
        public ActionResult AddExpertOpinion(string nameTable,string name, int pk, string value)
        {
            string msg = string.Empty;
            string status = "success";

            if (value.Length > 6)
            {
                status = "error";
                msg = "Значение не может быть длиннее 6 символов";
            }
            else
            {
                value = value.Replace(",", ".");
                UpdateTable(name, pk, value);
                Get_Q_D(true);
                ExpertTable();
            }

            return Json(new { status = status, msg = msg });
           
        }
      
        [HttpGet]
        public ActionResult TibetanQuestionForm(int index)
        {
            if (Session["TAQuestionsBase"] == null)
            {
                TibetanTestData questions = new TibetanTestData();
                Session["TAQuestionsBase"] = questions.TAQuestionsBase;
                return View(questions.TAQuestionsBase.FirstOrDefault(p => p.Index == index));
            }
            else
            {
                List<TibetanQuestionsModel> TAQuestionsBase = (List<TibetanQuestionsModel>)Session["TAQuestionsBase"];
                return View(TAQuestionsBase.FirstOrDefault(p => p.Index == index));
            }
        }

        [HttpPost]
        public ActionResult TibetanQuestionForm(int index, int tibetAnsw)
        {
            if (tibetAnsw == 0) UserAnswer.FAnsw = UserAnswer.FAnsw + 1;
            if (tibetAnsw == 1) UserAnswer.SAnsw = UserAnswer.SAnsw + 1;
            if (tibetAnsw == 2) UserAnswer.TAnsw = UserAnswer.TAnsw + 1;

            if (index != 25)
            {
                return RedirectToAction("TibetanQuestionForm", new { index = index + 1 });
            }
            else
            {
                data = CalculateResult();
                Session["data"] = data;
                return RedirectToAction("TibetanResult");  
            }


        }

       [HttpGet]
        public ActionResult TibetanResult()
        {
            data = Session["data"].ToString();
            return View((object)data);
        }

        [HttpPost]
        public ActionResult TibetanResult(int index)
        {

            if (Request.Form["nextTest"] != null)
            {
                // добавляем коэффициент
                SaveCoefficient();
                return RedirectToAction("QuestionForm", new { index = 0 });

            }
            else
                ClearTibetanTestRes();
            return RedirectToAction("TibetanQuestionForm", new { index = 0 });


        }

        public void SaveCoefficient()
        {
            using (SqlConnection con = new SqlConnection(strConnString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "SetCoeff_D";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    try
                    {
                        if (con.State != ConnectionState.Open)
                            con.Open();
                        cmd.Parameters.AddWithValue("@Mark", Convert.ToInt32(Session["mark"]));
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                    catch (Exception expt)
                    {
                        throw new Exception(expt.Message);
                    }
                }
            }
        }

        [HttpPost]
        public ActionResult ShowResult(int index)
        {

            if (Request.Form["again"] != null) //Начать заново
            {
                ClearTestRes();
                return RedirectToAction("QuestionForm", new { index = 0 });

            }
            else // Продолжить
                return RedirectToAction("QuestionForm", new { index = 0 }); 

        }

        public string CalculateResult()
        {
            double p1 = UserAnswer.FAnsw / 0.26; //0.26
            double p2 = UserAnswer.SAnsw / 0.26;
            double p3 = UserAnswer.TAnsw / 0.26;
            string result = "";
            int mark=0;
            //Первая группа
            if (p1 > p2 + p3) { result = "У Вас явно преобладают ответы по типу Ветер " + Math.Round(p1, 2).ToString() + "%"; mark = 1; }
            if (p2 > p1 + p3) { result = "У Вас явно преобладают ответы по типу  Желчь " + Math.Round(p2, 2).ToString() + "%"; mark = 2; }
            if (p3 > p2 + p1) { result = "У Вас явно преобладают ответы по типу  Слизь " + Math.Round(p3, 2).ToString() + "%"; mark = 3; }
            //Втотрая группа
            if (result =="")
            { //было 34
                if (p2 > 30 && p2 < 69 && p3 > 30 && p3 < 69) { result = "У Вас определен смешенный тип, преобладают типы: Желчь " + Math.Round(p2, 2).ToString() + " % и Слизь " + Math.Round(p3, 2).ToString() + "%"; mark = 4; }
                if (p1 > 30 && p1 < 69 && p3 > 30 && p3 < 69) {result = "У Вас определен смешенный тип, преобладают типы: Ветер " + Math.Round(p1, 2).ToString() + " % и Слизь " + Math.Round(p3, 2).ToString() + "%"; mark = 5;}
                if (p1 > 30 && p1 < 69 && p2 > 30 && p2 < 69) { result = "У Вас определен смешенный тип, преобладают типы: Ветер " + Math.Round(p1, 2).ToString() + " % и Желчь " + Math.Round(p2, 2).ToString() + "%"; mark = 6; }
              
            }
            if (p1 == 0 && p2 == 0 && p3 == 0) result = "Результат не определен";
           // if (result == "") 
            if (p1 < 30 && p2 < 30 && p3 < 30)
            {result = "У вас примерно одинаковы все три типа: Ветер - " + Math.Round(p1, 2).ToString() + "% Желчь - " + Math.Round(p2, 2).ToString() + "% Слизь- " + Math.Round(p3, 2).ToString() + "%"; mark = 7;}
           //Либо использовать флаг, в случае, если финальное выржение может быть и неверным
            Session["mark"] = mark;
            return result;
        }

        [HttpGet]
        public ActionResult QuestionForm(int index)
        {
           
           QuestionsContent questions = new QuestionsContent();
           idSymp = questions.idSympt;
           Session["idSymp"] = idSymp;
           index = questions.Numb -1; // продолжить диагностику
           Get_Q_D(false);
           return View(questions.QuestionsBase.FirstOrDefault(p=>p.Index==index));

        }

        public void Get_Q_D(bool i)
        {
         DataTable tblQ = new DataTable();
         DataTable tblD = new DataTable();
         DataSet ds = new DataSet();
             using (SqlConnection con = new SqlConnection(strConnString))
             {
                 using (SqlCommand cmdd = new SqlCommand())
                 {
                     if(i) cmdd.CommandText = "Get_QuestionsDesiases";// если true тогда выкачиваем полный список болезней и вопросов
                     else cmdd.CommandText = "Get_InformativeQuestionsDesiases"; //информативные болезнь-вопрос
                     cmdd.CommandType = CommandType.StoredProcedure;
                     try
                     {
                         if (con.State != ConnectionState.Open)
                             con.Open();
                         cmdd.Connection = con;
                         using (SqlDataAdapter da = new SqlDataAdapter(cmdd))
                         {

                             da.Fill(ds);
                         }
                         con.Close();
                     }
                     catch (Exception expt)
                     {
                         throw new Exception(expt.Message);
                     }

                 }
                 tblQ = ds.Tables[0];
                 tblD = ds.Tables[1];
                 QuestionsDataDefinitionModel.Queastions = tblQ.AsEnumerable().Select(x => Convert.ToString(x[0])).ToArray();
                 QuestionsDataDefinitionModel.Diagnosis = tblD.AsEnumerable().Select(x => Convert.ToString(x[0])).ToArray();
             }
        }

        [HttpPost]
        public ActionResult QuestionForm(int index, int answer, string code, int? criteri)
        {
            if (index == 0) Session["criteri"] = criteri;
            else
            { 
                int criteriNow =Convert.ToInt32(Session["criteri"]);
                if (criteriNow != criteri && criteri!=null) Session["criteri"] = criteri;
            }
            if (answer != 3)
            {
                idSymp = Convert.ToInt32(code.Substring(0, code.IndexOf("-")));
                //insert PROB_D             
                GetProbabilityD(answer);
                if (index == 10) StoppingCriteria();

                // if (index != 10) // критерий останова
                if (!stop)
                //#4 Если спустя 10 шагов (index) мы так и не пришли к критерию остановки, либо рельтат не удовлетворил пользователя,
                // меняем поведение системы - задаем вопросы по: Симптомам наиболее вероятного заболевания (вот тут сложнее будет), если и это не помогло, выводим все, что есть
                {
                    return RedirectToAction("QuestionForm", new { index = index + 1 });
                }
                else
                {
                    if (!stop)
                    {
                        // Инициируем процесс перерасчета, как по кнопке "Продолжить диагностику"?
                    }

                  //  GetListForGraph();
                    Session["data_res"] = dsResult;
                    return RedirectToAction("ShowResult", dsResult);


                }
            }
            else
            {
                GetFinalResult();
             //   GetListForGraph();
                Session["data_res"] = dsResult;
                return RedirectToAction("ShowResult", dsResult);
            }
        }

        public ActionResult Documentation()
        {
            return View();
        }
    
        public ActionResult ShowResult()
        {
          //  if (tblResult != null) ??

            dsResult = (DataSet)Session["data_res"]; 
            return View(dsResult);
        }

        public void GetProbabilityD(int answer)
        {
            //idSymp = Convert.ToInt32(Session["idSymp"]);
            using (SqlConnection con = new SqlConnection(strConnString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "GetProbD";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Answer", SqlDbType.Int);
                    cmd.Parameters.Add("@IDSympt", SqlDbType.Int); 
                    try
                    {
                        if (con.State != ConnectionState.Open)
                            con.Open();
                        cmd.Connection = con;
                        cmd.Parameters["@Answer"].Value = answer;
                        cmd.Parameters["@IDSympt"].Value = idSymp;
                        cmd.ExecuteNonQuery();
                        con.Close();
                   
                    }
                    catch (Exception expt)
                    {
                        throw new Exception(expt.Message);
                    }
                 
                  
                    
                }
            }

        }


        //#1 Вероятность одной из болезней стала выше 0,8
        //#2 Считаем по таблице, чья вероятность на протяжении всего теста росла (падала?) - >80% ответов
        //#3 Останавливаемся на группе самых вероятных заболеваний (допустим выше 0,6)

        public void StoppingCriteria()
        {
            double maxval;
            int i=0;
            int criteriNow = Convert.ToInt32(Session["criteri"]);
            GetFinalResult();
            maxval = Convert.ToDouble(dsResult.Tables[0].Rows[0]["val"]);      // заменить на 0!
            if (maxval > 0.8 && criteriNow == 1) //0.8!                  
                stop = true;
            else
            {
                if (criteriNow == 2)
                {
                    foreach (DataRow row in dsResult.Tables[0].Rows)
                    {
                        maxval = Convert.ToDouble(row["val"]);
                        if (maxval > 0.6) i++;
                    }
                    if (i >= 2) stop = true; // если хотя бы в 2x диагнозах вероятность больше 0.6 (изменить 2 на 5)
                }
                else
                    stop = false;
            }
         
        }
        public void GetFinalResult()
        {
            dsResult = new DataSet("dsResult");
            using (SqlConnection con = new SqlConnection(strConnString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "Get_FinalData";
                    cmd.CommandType = CommandType.StoredProcedure;
                    try
                    {
                        if (con.State != ConnectionState.Open)
                            con.Open();
                        cmd.Connection = con;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dsResult);
                        }
                        con.Close();
                    }
                    catch (Exception expt)
                    {
                        throw new Exception(expt.Message);
                    }
                }
            }
        }

        private DataTable GetTable(DataTable schemaTable)
        {
            if (schemaTable == null || schemaTable.Rows.Count == 0)
            {
                return null;
            }

            var dt = new DataTable();
            foreach (DataRow schemaRow in schemaTable.Rows)
            {
                var col = new DataColumn
                {
                    ColumnName = schemaRow["ColumnName"].ToString(),
                    DataType = System.Type.GetType(schemaRow["DataType"].ToString())
                };
                dt.Columns.Add(col);
            }
            return dt;

        }

        public void GetListForGraph()
        {
            DataSet ds = new DataSet();
            tblFinal = new DataTable("Data");
            DataTable tblEntropy = new DataTable("DataEntropy");
            //Number, id, disease,val
            using (SqlConnection con = new SqlConnection(strConnString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "Get_FinalTable";
                    cmd.CommandType = CommandType.StoredProcedure;
                    try
                    {
                        if (con.State != ConnectionState.Open)
                            con.Open();
                        cmd.Connection = con;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                           
                            da.Fill(ds);
                          //  da.Fill(tblFinal);
                        }
                        con.Close();
                    }
                    catch (Exception expt)
                    {
                        throw new Exception(expt.Message);
                    }
                }
            }

            tblFinal = ds.Tables[0];
            tblEntropy = ds.Tables[1];

            UserAnswer.List1 = tblFinal.AsEnumerable().Select(x => Convert.ToDecimal(x[0])).ToArray();
            UserAnswer.List2 = tblFinal.AsEnumerable().Select(x => Convert.ToDecimal(x[1])).ToArray();
            UserAnswer.List3 = tblFinal.AsEnumerable().Select(x => Convert.ToDecimal(x[2])).ToArray();
            UserAnswer.List4 = tblFinal.AsEnumerable().Select(x => Convert.ToDecimal(x[3])).ToArray();
            UserAnswer.List5 = tblFinal.AsEnumerable().Select(x => Convert.ToDecimal(x[4])).ToArray();

            UserAnswer.ListEntropy1 = tblEntropy.AsEnumerable().Select(x => Convert.ToDecimal(x[0])).ToArray();
            UserAnswer.ListEntropy2 = tblEntropy.AsEnumerable().Select(x => Convert.ToDecimal(x[1])).ToArray();
            UserAnswer.ListEntropy3 = tblEntropy.AsEnumerable().Select(x => Convert.ToDecimal(x[2])).ToArray();
            UserAnswer.ListEntropy4 = tblEntropy.AsEnumerable().Select(x => Convert.ToDecimal(x[3])).ToArray();
            UserAnswer.ListEntropy5 = tblEntropy.AsEnumerable().Select(x => Convert.ToDecimal(x[4])).ToArray();

        }
    }
}