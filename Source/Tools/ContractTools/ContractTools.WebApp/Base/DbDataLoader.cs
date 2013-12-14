/****************************************************************************
Copyright (c) 2013-2015 scutgame.com

http://www.scutgame.com

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
****************************************************************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ContractTools.WebApp.Model;
using ZyGames.Framework.Common;
using ZyGames.Framework.Data;

namespace ContractTools.WebApp.Base
{
    /// <summary>
    /// The db data loader
    /// </summary>
    public static class DbDataLoader
    {
        private static DbBaseProvider _dbBaseProvider;

        static DbDataLoader()
        {
            _dbBaseProvider = DbConnectionProvider.CreateDbProvider("Contract");
        }

        #region SolutionModel

        public static int Add(SolutionModel model)
        {
            var command = _dbBaseProvider.CreateCommandStruct("Solutions", CommandMode.Insert);
            command.AddParameter("SlnName", model.SlnName);
            command.AddParameter("Namespace", model.Namespace);
            command.AddParameter("Url", model.Url);
            command.AddParameter("GameID", model.GameID);
            command.ReturnIdentity = true;
            command.Parser();
            return _dbBaseProvider.ExecuteQuery(CommandType.Text, command.Sql, command.Parameters);

        }
        public static bool Update(SolutionModel model)
        {
            var command = _dbBaseProvider.CreateCommandStruct("Solutions", CommandMode.Modify);
            command.AddParameter("SlnName", model.SlnName);
            command.AddParameter("Namespace", model.Namespace);
            command.AddParameter("Url", model.Url);
            command.AddParameter("GameID", model.GameID);
            command.Filter = _dbBaseProvider.CreateCommandFilter();
            command.Filter.Condition = _dbBaseProvider.FormatFilterParam("SlnID");
            command.Filter.AddParam("SlnID", model.SlnID);
            command.Parser();
            return _dbBaseProvider.ExecuteQuery(CommandType.Text, command.Sql, command.Parameters) > 0;

        }
        /// <summary>
        /// ɾ��һ������
        /// </summary>
        public static bool Delete(SolutionModel model)
        {
            var command = _dbBaseProvider.CreateCommandStruct("Solutions", CommandMode.Delete);
            command.Filter = _dbBaseProvider.CreateCommandFilter();
            command.Filter.Condition = _dbBaseProvider.FormatFilterParam("SlnID");
            command.Filter.AddParam("SlnID", model.SlnID);
            command.Parser();
            return _dbBaseProvider.ExecuteQuery(CommandType.Text, command.Sql, command.Parameters) > 0;
        }
        public static SolutionModel GetSolution(int slnId)
        {
            return GetSolution(f =>
            {
                f.Condition = f.FormatExpression("SlnID");
                f.AddParam("SlnID", slnId);
            }).FirstOrDefault();
        }

        public static List<SolutionModel> GetSolution(Action<CommandFilter> match)
        {
            var command = _dbBaseProvider.CreateCommandStruct("Solutions", CommandMode.Inquiry);
            command.Columns = "SlnID,SlnName,Namespace,RefNamespace,Url,GameID";
            command.OrderBy = "SlnID ASC";
            command.Filter = _dbBaseProvider.CreateCommandFilter();
            if (match != null)
            {
                match(command.Filter);
            }
            command.Parser();
            var list = new List<SolutionModel>();
            using (var reader = _dbBaseProvider.ExecuteReader(CommandType.Text, command.Sql, command.Parameters))
            {
                while (reader.Read())
                {
                    SolutionModel model = new SolutionModel();
                    model.SlnID = reader["SlnID"].ToInt();
                    model.GameID = reader["GameID"].ToInt();
                    model.SlnName = reader["SlnName"].ToNotNullString();
                    model.Namespace = reader["Namespace"].ToNotNullString();
                    model.RefNamespace = reader["RefNamespace"].ToNotNullString();
                    model.Url = reader["Url"].ToNotNullString();
                    list.Add(model);
                }
            }
            return list;
        }
        #endregion

        #region AgreementModel
        public static int Add(AgreementModel model)
        {
            var command = _dbBaseProvider.CreateCommandStruct("AgreementClass", CommandMode.Insert);
            command.AddParameter("GameID", model.GameID);
            command.AddParameter("Title", model.Title);
            command.AddParameter("Describe", model.Describe);
            command.ReturnIdentity = true;
            command.Parser();
            using (var reader = _dbBaseProvider.ExecuteReader(CommandType.Text, command.Sql, command.Parameters))
            {
                if (reader.Read())
                {
                    model.AgreementID = reader[0].ToInt();
                }
            }
            return model.AgreementID;
        }

        public static bool Update(AgreementModel model)
        {
            var command = _dbBaseProvider.CreateCommandStruct("AgreementClass", CommandMode.Modify);
            command.AddParameter("Title", model.Title);
            command.AddParameter("Describe", model.Describe);
            command.Filter = _dbBaseProvider.CreateCommandFilter();
            command.Filter.Condition = _dbBaseProvider.FormatFilterParam("AgreementID");
            command.Filter.AddParam("AgreementID", model.AgreementID);
            command.Parser();
            return _dbBaseProvider.ExecuteQuery(CommandType.Text, command.Sql, command.Parameters) > 0;
        }

        public static bool Delete(AgreementModel model)
        {
            var command = _dbBaseProvider.CreateCommandStruct("AgreementClass", CommandMode.Delete);
            command.Filter = _dbBaseProvider.CreateCommandFilter();
            command.Filter.Condition = _dbBaseProvider.FormatFilterParam("AgreementID");
            command.Filter.AddParam("AgreementID", model.AgreementID);
            command.Parser();
            return _dbBaseProvider.ExecuteQuery(CommandType.Text, command.Sql, command.Parameters) > 0;
        }

        public static List<AgreementModel> GetAgreement(int gameId)
        {
            return GetAgreement(f =>
            {
                f.Condition = f.FormatExpression("GameID");
                f.AddParam("GameID", gameId);
            });
        }

        public static List<AgreementModel> GetAgreement(Action<CommandFilter> match)
        {
            var command = _dbBaseProvider.CreateCommandStruct("AgreementClass", CommandMode.Inquiry);
            command.Columns = string.Format("AgreementID,GameID,Title,Describe,CreateDate,(SELECT SLNNAME FROM SOLUTIONS WHERE SLNID={0}.GAMEID) SlnName", command.TableName);
            command.OrderBy = "AgreementID ASC";
            command.Filter = _dbBaseProvider.CreateCommandFilter();
            if (match != null)
            {
                match(command.Filter);
            }
            command.Parser();
            var list = new List<AgreementModel>();
            using (var reader = _dbBaseProvider.ExecuteReader(CommandType.Text, command.Sql, command.Parameters))
            {
                while (reader.Read())
                {
                    AgreementModel model = new AgreementModel();
                    model.AgreementID = reader["AgreementID"].ToInt();
                    model.GameID = reader["GameID"].ToInt();
                    model.Title = reader["Title"].ToNotNullString();
                    model.Describe = reader["Describe"].ToNotNullString();
                    model.CreateDate = reader["CreateDate"].ToDateTime();
                    model.SlnName = reader["SlnName"].ToNotNullString();

                    list.Add(model);
                }
            }
            return list;
        }

        #endregion

        #region ContractModel
        public static int Add(ContractModel model)
        {
            var command = _dbBaseProvider.CreateCommandStruct("Contract", CommandMode.Insert);
            command.AddParameter("ID", model.ID);
            command.AddParameter("Descption", model.Descption);
            command.AddParameter("ParentID", model.ParentID);
            command.AddParameter("SlnID", model.SlnID);
            command.AddParameter("Complated", model.Complated);
            command.AddParameter("AgreementID", model.AgreementID);
            command.Parser();
            return _dbBaseProvider.ExecuteQuery(CommandType.Text, command.Sql, command.Parameters);
        }

        public static bool Update(ContractModel model)
        {
            var command = _dbBaseProvider.CreateCommandStruct("Contract", CommandMode.Modify);
            command.AddParameter("Descption", model.Descption);
            command.AddParameter("ParentID", model.ParentID);
            command.AddParameter("Complated", model.Complated);
            command.AddParameter("AgreementID", model.AgreementID);
            command.Filter = _dbBaseProvider.CreateCommandFilter();
            command.Filter.Condition = string.Format("{0} AND {1}",
                _dbBaseProvider.FormatFilterParam("ID"),
                _dbBaseProvider.FormatFilterParam("SlnID"));
            command.Filter.AddParam("ID", model.ID);
            command.Filter.AddParam("SlnID", model.SlnID);
            command.Parser();
            return _dbBaseProvider.ExecuteQuery(CommandType.Text, command.Sql, command.Parameters) > 0;
        }

        public static bool Delete(ContractModel model)
        {
            var command = _dbBaseProvider.CreateCommandStruct("Contract", CommandMode.Delete);
            command.Filter = _dbBaseProvider.CreateCommandFilter();
            command.Filter.Condition = string.Format("{0} AND {1}",
                _dbBaseProvider.FormatFilterParam("ID"),
                _dbBaseProvider.FormatFilterParam("SlnID"));
            command.Filter.AddParam("ID", model.ID);
            command.Filter.AddParam("SlnID", model.SlnID);
            command.Parser();
            return _dbBaseProvider.ExecuteQuery(CommandType.Text, command.Sql, command.Parameters) > 0;
        }

        public static List<ContractModel> GetContract(int slnId)
        {
            return GetContract(f =>
            {
                f.Condition = f.FormatExpression("SlnID");
                f.AddParam("SlnID", slnId);
            });
        }

        public static List<ContractModel> GetContract(int slnId, int contractId)
        {
            return GetContract(f =>
            {
                f.Condition = string.Format("{0} AND {1}",
                    f.FormatExpression("ID"),
                    f.FormatExpression("SlnID"));
                f.AddParam("ID", contractId);
                f.AddParam("SlnID", slnId);
            });
        }

        public static List<ContractModel> GetContract(Action<CommandFilter> match)
        {
            var command = _dbBaseProvider.CreateCommandStruct("Contract", CommandMode.Inquiry);
            command.Columns = "ID,Descption,ParentID,SlnID,Complated,AgreementID,(Descption) as uname";
            command.OrderBy = "SlnID ASC,ID ASC";
            command.Filter = _dbBaseProvider.CreateCommandFilter();
            if (match != null)
            {
                match(command.Filter);
            }
            command.Parser();
            var list = new List<ContractModel>();
            using (var reader = _dbBaseProvider.ExecuteReader(CommandType.Text, command.Sql, command.Parameters))
            {
                while (reader.Read())
                {
                    ContractModel model = new ContractModel();
                    model.ID = reader["ID"].ToInt();
                    model.Descption = reader["Descption"].ToNotNullString();
                    model.ParentID = reader["ParentID"].ToInt();
                    model.SlnID = reader["SlnID"].ToInt();
                    model.Complated = reader["Complated"].ToBool();
                    model.AgreementID = reader["AgreementID"].ToInt();

                    list.Add(model);
                }
            }
            return list;
        }

        #endregion

        #region ParamInfoModel
        public static int Add(ParamInfoModel model)
        {
            var command = _dbBaseProvider.CreateCommandStruct("ParamInfo", CommandMode.Insert);
            command.ReturnIdentity = true;
            command.AddParameter("SlnID", model.SlnID);
            command.AddParameter("ContractID", model.ContractID);
            command.AddParameter("ParamType", model.ParamType);
            command.AddParameter("Field", model.Field);
            command.AddParameter("FieldType", model.FieldType);
            command.AddParameter("Descption", model.Descption);
            command.AddParameter("FieldValue", model.FieldValue);
            command.AddParameter("Required", model.Required);
            command.AddParameter("Remark", model.Remark);
            command.AddParameter("SortID", model.SortID);
            command.AddParameter("Creator", model.Creator);
            command.AddParameter("Modifier", model.Modifier);
            command.AddParameter("MinValue", model.MinValue);
            command.AddParameter("MaxValue", model.MaxValue);
            command.Parser();
            return _dbBaseProvider.ExecuteQuery(CommandType.Text, command.Sql, command.Parameters);
        }

        public static bool Update(ParamInfoModel model)
        {
            var command = _dbBaseProvider.CreateCommandStruct("ParamInfo", CommandMode.Modify);
            command.AddParameter("SlnID", model.SlnID);
            command.AddParameter("ContractID", model.ContractID);
            command.AddParameter("ParamType", model.ParamType);
            command.AddParameter("Field", model.Field);
            command.AddParameter("FieldType", model.FieldType);
            command.AddParameter("Descption", model.Descption);
            command.AddParameter("FieldValue", model.FieldValue);
            command.AddParameter("Required", model.Required);
            command.AddParameter("Remark", model.Remark);
            command.AddParameter("SortID", model.SortID);
            command.AddParameter("Creator", model.Creator);
            command.AddParameter("Modifier", model.Modifier);
            command.AddParameter("MinValue", model.MinValue);
            command.AddParameter("MaxValue", model.MaxValue);
            command.Filter = _dbBaseProvider.CreateCommandFilter();
            command.Filter.Condition = _dbBaseProvider.FormatFilterParam("ID");
            command.Filter.AddParam("ID", model.ID);
            command.Parser();
            return _dbBaseProvider.ExecuteQuery(CommandType.Text, command.Sql, command.Parameters) > 0;
        }

        public static bool Delete(ParamInfoModel model)
        {
            var command = _dbBaseProvider.CreateCommandStruct("ParamInfo", CommandMode.Delete);
            command.Filter = _dbBaseProvider.CreateCommandFilter();
            command.Filter.Condition = _dbBaseProvider.FormatFilterParam("ID");
            command.Filter.AddParam("ID", model.ID);
            command.Parser();
            return _dbBaseProvider.ExecuteQuery(CommandType.Text, command.Sql, command.Parameters) > 0;
        }

        public static List<ParamInfoModel> GetParamInfo(int slnId, int contractId, int paramType)
        {
            return GetParamInfo(f =>
            {
                f.Condition = string.Format("{0} AND {1} AND {2}",
                    f.FormatExpression("ContractID"),
                    f.FormatExpression("SlnID"),
                    f.FormatExpression("ParamType"));
                f.AddParam("ContractID", contractId);
                f.AddParam("SlnID", slnId);
                f.AddParam("ParamType", paramType);
            });
        }

        public static List<ParamInfoModel> GetParamInfo(int slnId, int contractId)
        {
            return GetParamInfo(f =>
            {
                f.Condition = string.Format("{0} AND {1}",
                    f.FormatExpression("ContractID"),
                    f.FormatExpression("SlnID"));
                f.AddParam("ContractID", contractId);
                f.AddParam("SlnID", slnId);
            });
        }

        public static List<ParamInfoModel> GetParamInfo(Action<CommandFilter> match)
        {
            var command = _dbBaseProvider.CreateCommandStruct("ParamInfo", CommandMode.Inquiry);
            command.Columns = "ID,SlnID,ContractID,ParamType,Field,FieldType,Descption,FieldValue,Required,Remark,SortID,Creator,CreateDate,Modifier,ModifyDate,MinValue,MaxValue";
            command.OrderBy = "PARAMTYPE ASC,SORTID ASC,ID ASC";
            command.Filter = _dbBaseProvider.CreateCommandFilter();
            if (match != null)
            {
                match(command.Filter);
            }
            command.Parser();
            var list = new List<ParamInfoModel>();
            using (var reader = _dbBaseProvider.ExecuteReader(CommandType.Text, command.Sql, command.Parameters))
            {
                while (reader.Read())
                {
                    ParamInfoModel model = new ParamInfoModel();
                    model.ID = reader["ID"].ToInt();
                    model.SlnID = reader["SlnID"].ToInt();
                    model.ContractID = reader["ContractID"].ToInt();
                    model.ParamType = reader["ParamType"].ToInt();
                    model.Field = reader["Field"].ToNotNullString();
                    model.FieldType = reader["FieldType"].ToInt();
                    model.Descption = reader["Descption"].ToNotNullString();
                    model.FieldValue = reader["FieldValue"].ToNotNullString();
                    model.Required = reader["Required"].ToBool();
                    model.Remark = reader["Remark"].ToNotNullString();
                    model.SortID = reader["SortID"].ToInt();
                    model.Creator = reader["Creator"].ToInt();
                    model.CreateDate = reader["CreateDate"].ToDateTime();
                    model.Modifier = reader["Modifier"].ToInt();
                    model.ModifyDate = reader["ModifyDate"].ToDateTime();
                    model.MinValue = reader["MinValue"].ToInt();
                    model.MaxValue = reader["MaxValue"].ToInt();

                    list.Add(model);
                }
            }
            return list;
        }

        #endregion

    }
}