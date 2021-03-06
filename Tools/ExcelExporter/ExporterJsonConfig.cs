﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.Core;
using MongoDB.Bson;
using NiceET;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace ExcelExporter
{
    public class ExporterJsonConfig
    {
        private const string ExcelPath = "../Excel";
        private const string JsonPath = "../output_json";
        private const string IdlPath = "../output_idl";
        private const string LuaPath = "../output_lua/fb";
        private const string TsPath = "../output_ts";

        private List<string> ignoreFiles = new List<string>()
        {
            "StartMachineConfig.xlsx",
            "StartProcessConfig.xlsx",
            "StartSceneConfig.xlsx",
            "StartZoneConfig.xlsx",

        };

        //变量以下划线开头
        private string underscope_field(string field)
        {
            string res = field;

            if (!field.StartsWith("_"))
            {
                res = $"_{field}";
            }
            return res;
        }

        public void ExportJsonConfig()
        {
            try
            {
                ExportAll(JsonPath);

                ExportAllIDLS(IdlPath);

                ExportData();

                GenerateLuaUtilsFiles();

                GenerateTSMgr();

                GenerateTSData();

                Console.WriteLine($"导出服务端配置完成!");
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
            
        }

        private void GenerateTSMgr()
        {
            string tsMgrPath = Path.Combine(TsPath, "ExcelManager.ts");
            StringBuilder sbMgrHeader = new StringBuilder();
            sbMgrHeader.Append("import { Singleton } from \"../../framework/common/Singleton\";\n");
            

            StringBuilder sbMgrBody = new StringBuilder();
            sbMgrBody.Append("export class ExcelManager extends Singleton<ExcelManager>{\n");
            sbMgrBody.Append("\tconstructor(){\n");
            sbMgrBody.Append("\t\tsuper();\n");

            foreach (string filePath in Directory.GetFiles(ExcelPath))
            {
                if (Path.GetExtension(filePath) != ".xlsx")
                {
                    continue;
                }

                if (Path.GetFileName(filePath).StartsWith("~"))
                {
                    continue;
                }

                if (ignoreFiles.Contains(Path.GetFileName(filePath)))
                {
                    continue;
                }

                string filename = Path.GetFileNameWithoutExtension(filePath);
                string filenameUpperFirst = filename.Substring(0, 1).ToUpper() + filename.Substring(1);

                sbMgrHeader.Append($"import {{ {filenameUpperFirst}TB }} from \"./{filename}\";\n");
                sbMgrBody.Append($"\t\t{filenameUpperFirst}TB.Instance({filenameUpperFirst}TB);\n");
            }
            

            sbMgrBody.Append("\t}\n");
            sbMgrBody.Append(" }\n");

            using (FileStream txt = new FileStream(tsMgrPath, FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(txt))
                {
                    sw.Write(sbMgrHeader.ToString());
                    sw.Write(sbMgrBody.ToString());
                    sw.Flush();
                }
            }
        }

        private void GenerateTSData()
        {
            foreach (string filePath in Directory.GetFiles(ExcelPath))
            {
                if (Path.GetExtension(filePath) != ".xlsx") continue; 

                if (Path.GetFileName(filePath).StartsWith("~")) continue; 

                if (ignoreFiles.Contains(Path.GetFileName(filePath))) continue;
            

                string filename = Path.GetFileNameWithoutExtension(filePath);
                string filenameUpperFirst = filename.Substring(0, 1).ToUpper() + filename.Substring(1);

                

                XSSFWorkbook xssfWorkbook;
                using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    xssfWorkbook = new XSSFWorkbook(file);


                    string tsdataPath = Path.Combine(TsPath, $"{filename}.ts");
                    StringBuilder sbTR = new StringBuilder();
                    StringBuilder sbTRProp = new StringBuilder();
                    StringBuilder sbTRParams = new StringBuilder();
                    StringBuilder sbTRInit = new StringBuilder();

                    StringBuilder sbTB = new StringBuilder();

                    //生成TR
                    sbTR.Append("import { Singleton } from \"../../framework/common/Singleton\";\n");
                    sbTR.Append($"export class {filenameUpperFirst}TR{{");


                    //生成TB
                    sbTB.Append($"export class {filenameUpperFirst}TB extends Singleton<{filenameUpperFirst}TB>{{ \n");
                    sbTB.Append($"\tpublic trs:Map<number, {filenameUpperFirst}TR> = new Map<number, {filenameUpperFirst}TR>();\n");
                    sbTB.Append("\tconstructor(){\n");
                    sbTB.Append("\t\tsuper();\n");

                   // for (int k = 0; k < xssfWorkbook.NumberOfSheets; ++k)
                    {
                        int k = 0; //只支持一个sheet
                        ISheet sheet = xssfWorkbook.GetSheetAt(k);

                        int cellCount = sheet.GetRow(0).LastCellNum;

                        CellInfo[] cellInfos = new CellInfo[cellCount];

                        for (int i = 0; i < cellCount; i++)
                        {
                            string fieldDesc = ExcelHelper.GetCellString(sheet, 0, i);
                            string fieldName = ExcelHelper.GetCellString(sheet, 1, i);
                            string fieldType = ExcelHelper.GetCellString(sheet, 2, i);
                            cellInfos[i] = new CellInfo() { Name = fieldName, Type = fieldType, Desc = fieldDesc };

                            sbTRProp.Append($"\t public {fieldName}:{GetTSType(fieldType)} ;\n");

                            if (i == 0)
                            {
                                sbTRParams.Append($"{fieldName}:{GetTSType(fieldType)}");
                            }
                            else
                            {
                                sbTRParams.Append($", {fieldName}:{GetTSType(fieldType)}");
                            }
                            sbTRInit.Append($"\t\tthis.{fieldName} = {fieldName};\n");
                        }

                        for (int i = 3; i <= sheet.LastRowNum; ++i)
                        {
                           
                            if (ExcelHelper.GetCellString(sheet, i, 0) == "")
                            {
                                break; ;
                            }

                            IRow row = sheet.GetRow(i);
                            string id = ExcelHelper.GetCellString(row, 0);
                            sbTB.Append($"\t\tthis.trs.set({id}, new {filenameUpperFirst}TR(");

                            for (int j = 0; j < cellCount; ++j)
                            {
                                string fieldValue = ExcelHelper.GetCellString(row, j);
                                string fieldType = cellInfos[j].Type;
                                if(fieldValue== null || fieldValue.Trim() == "")
                                {
                                    fieldValue = GetTSDefaultValue(fieldType);
                                }
                                fieldValue = Convert(fieldType, fieldValue);
                                if (j == 0)
                                {
                                    sbTB.Append($"{fieldValue}");

                                }
                                else
                                {
                                    sbTB.Append($", {fieldValue}");
                                }
             

                            }
                            sbTB.Append($"));\n");

                        }


                        using (FileStream txt = new FileStream(tsdataPath, FileMode.Create))
                        using (StreamWriter sw = new StreamWriter(txt))
                        {
                            //生成TR类
                            sw.WriteLine(sbTR.ToString());
                            sw.WriteLine(sbTRProp.ToString());
                            sw.Write("\tconstructor(");
                            sw.Write(sbTRParams.ToString());
                            sw.WriteLine("){");
                            sw.WriteLine(sbTRInit.ToString());
                            sw.WriteLine("\t}");
                            sw.WriteLine("}\n");



                            //生成TB类
                            sbTB.Append("\t }\n");
                            sbTB.Append("}\n");
                            sw.WriteLine(sbTB.ToString());
                            sw.Flush();
                        }
                    }


                    

                }


                
            }
        }


        private void GenerateLuaUtilsFiles()
        {
            string luaMgrPath = Path.Combine(LuaPath, "FBMapManager.lua");
            StringBuilder sbMgr = new StringBuilder();
            sbMgr.Append("local FBMapManager = BaseClass(\"FBMapManager\", Singleton) \n");
            sbMgr.Append("local function __init(self)\n");

            foreach (string filePath in Directory.GetFiles(ExcelPath))
            {
                if (Path.GetExtension(filePath) != ".xlsx")
                {
                    continue;
                }

                if (Path.GetFileName(filePath).StartsWith("~"))
                {
                    continue;
                }

                if (ignoreFiles.Contains(Path.GetFileName(filePath)))
                {
                    continue;
                }

                string filename = Path.GetFileNameWithoutExtension(filePath).ToLower();
                string filenameUpperFirst = filename.Substring(0, 1).ToUpper() + filename.Substring(1);

                string luaUtilPath = Path.Combine(LuaPath, $"{filename}Util.lua");

                sbMgr.Append($"\trequire(\"fb.{filename}Util\"):GetInstance()\n");


                using(FileStream txt = new FileStream(luaUtilPath, FileMode.Create))
                {
                    using(StreamWriter sw = new StreamWriter(txt))
                    {
                        StringBuilder sbUtil = new StringBuilder();
                        sbUtil.Append($"local {filename}Util = BaseClass(\"{filename}Util\", Singleton)\n");
                        sbUtil.Append("local function __init(self)\n");
                        sbUtil.Append("\tself.map = {}\n");
                        sbUtil.Append($"\tlocal {filename}TB = require(\"fb.{filename}TB\")\n");
                        sbUtil.Append($"\tlocal tbbuf = FBUtil:GetInstance():GetFB(\"{filename}\")\n");
                        sbUtil.Append($"\tlocal tbObj = {filename}TB.GetRootAs{filename}TB(tbbuf, 0)\n");

                        sbUtil.Append($"\tfor i = 1, tbObj:{filenameUpperFirst}TRSLength() do\n");
                        sbUtil.Append($"\t\tlocal trObj = tbObj:{filenameUpperFirst}TRS(i);\n");
                        sbUtil.Append($"\t\tself.map[trObj:_id()] = trObj\n");
                        sbUtil.Append("\tend\n");//end for

                        sbUtil.Append("end\n"); //end init

                        sbUtil.Append("local function GetByID(self, id)\n");
                        sbUtil.Append("\treturn self.map[id]\n");
                        sbUtil.Append("end\n");

                        sbUtil.Append("local function GetAll(self)\n");
                        sbUtil.Append("\treturn self.map\n");
                        sbUtil.Append("end\n");

                        sbUtil.Append($"{filename}Util.__init = __init\n");
                        sbUtil.Append($"{filename}Util.GetByID = GetByID\n");
                        sbUtil.Append($"{filename}Util.GetAll = GetAll\n");
                        sbUtil.Append($"return {filename}Util\n");

                        sw.Write(sbUtil);
                        sw.Flush();
                    }
                }

            }

            sbMgr.Append("end\n");
            sbMgr.Append("FBMapManager.__init = __init\n");
            sbMgr.Append("return FBMapManager\n");

            using(FileStream txt = new FileStream(luaMgrPath, FileMode.Create))
            {
                using(StreamWriter sw = new StreamWriter(txt))
                {
                    sw.Write(sbMgr.ToString());
                    sw.Flush();
                }
            }
        }

        private void ExportData()
        {
            string exePath = Path.GetFullPath("../");

            foreach (string filePath in Directory.GetFiles(JsonPath))
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                string argvB      = $"-b -o output_bin output_idl/{fileName}.fbs  output_json/{fileName}.txt";
                string argvCsharp = $"-n --gen-onefile  -o output_csharp output_idl/{fileName}.fbs  output_json/{fileName}.txt";
                string argvLua    = $"-l -o output_lua output_idl/{fileName}.fbs  output_json/{fileName}.txt";
                string argvTS = $"--ts --no-fb-import --no-ts-reexport  -o output_fbts output_idl/{fileName}.fbs  output_json/{fileName}.txt";

                ProcessHelper.Run( $"{exePath}/flatc.exe",  argvB,  exePath);
                ProcessHelper.Run( $"{exePath}/flatc.exe", argvCsharp,  exePath);
                ProcessHelper.Run( $"{exePath}/flatc.exe", argvLua,  exePath);
                ProcessHelper.Run( $"{exePath}/flatc.exe", argvTS,  exePath);

                Console.WriteLine(argvB);
            }
            
        }

        public void ExportAllIDLS(string exportDir)
        {
            foreach (string filePath in Directory.GetFiles(ExcelPath))
            {
                if (Path.GetExtension(filePath) != ".xlsx")
                {
                    continue;
                }

                if (Path.GetFileName(filePath).StartsWith("~"))
                {
                    continue;
                }

                if (ignoreFiles.Contains(Path.GetFileName(filePath)))
                {
                    continue;
                }

                ExportIDL(filePath, exportDir);
                Console.WriteLine($"生成{Path.GetFileName(filePath)}Schema");
            }

     
        }

        private void ExportIDL(string fileName, string exportDir)
        {
            XSSFWorkbook xssfWorkbook;
            using (FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                xssfWorkbook = new XSSFWorkbook(file);
            }

            string protoName = Path.GetFileNameWithoutExtension(fileName).ToLower();

            string exportPath = Path.Combine(exportDir, $"{protoName}.fbs");

            using (FileStream txt = new FileStream(exportPath, FileMode.Create))
            using (StreamWriter sw = new StreamWriter(txt))
            {
                StringBuilder sb = new StringBuilder();
                ISheet sheet = xssfWorkbook.GetSheetAt(0);

                sb.Append("namespace fb; \n");

                //gen TB
                sb.Append($"table {protoName}TB\n");
                sb.Append("{\n");
                sb.Append($"\t {protoName}TRS:[{protoName}TR];\n");
                sb.Append("}\n\n"); //end TB

                //gen TR
                sb.Append($"table {protoName}TR\n");
                sb.Append("{\n");

                int cellCount = sheet.GetRow(0).LastCellNum;

                for (int i = 0; i < cellCount; i++)
                {
                    string fieldDesc = ExcelHelper.GetCellString(sheet, 0, i);

                    if (fieldDesc.StartsWith("#"))
                    {
                        continue;
                    }

                    string fieldName = ExcelHelper.GetCellString(sheet, 1, i).ToLower();


                    string fieldType = ExcelHelper.GetCellString(sheet, 2, i);
                    if (fieldType == "" || fieldName == "")
                    {
                        continue;
                    }
                    string idlType = Convert(fieldType);

                    //key
                    if (fieldName.Equals("_id"))
                    {
                        idlType += "(key)";
                    }

                    sb.Append($"\t {underscope_field(fieldName)}:{idlType};\n");
                }

                sb.Append("}\n"); //end TR

                sb.Append($"root_type {protoName}TB;");

                sw.Write(sb.ToString());
            }
        }
        private static string Convert(string type)
        {
            switch (type)
            {
                case "int[]":
                    return "[int]";
                case "int32[]":
                    return "[int32]";
                case "long[]":
                    return "[long]";
                case "string[]":
                    return "[string]";
                case "int":
                case "int32":
                case "int64":
                case "long":
                case "float":
                case "double":
                case "string":
                    return type;
                default:
                    throw new Exception($"不支持此类型: {type}");
            }
        }

        private void ExportAll(string exportDir)
        {


            foreach (string filePath in Directory.GetFiles(ExcelPath))
            {
                if (Path.GetExtension(filePath) != ".xlsx")
                {
                    continue;
                }

                if (Path.GetFileName(filePath).StartsWith("~"))
                {
                    continue;
                }

                if (ignoreFiles.Contains(Path.GetFileName(filePath)))
                {
                    continue;
                }
           

                Export(filePath, exportDir);
            }


            Console.WriteLine("所有表导表完成");
      
        }

        private void Export(string fileName, string exportDir)
        {
            XSSFWorkbook xssfWorkbook;
            using (FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                xssfWorkbook = new XSSFWorkbook(file);
            }

            string protoName = Path.GetFileNameWithoutExtension(fileName).ToLower();

            Console.WriteLine($"{protoName}导表开始");
            string exportPath = Path.Combine(exportDir, $"{protoName}.txt");
            using (FileStream txt = new FileStream(exportPath, FileMode.Create))
            using (StreamWriter sw = new StreamWriter(txt))
            {
                sw.WriteLine('{');
                sw.WriteLine($"\"{protoName}TRS\":[");
                for (int i = 0; i < xssfWorkbook.NumberOfSheets; ++i)
                {
                    ISheet sheet = xssfWorkbook.GetSheetAt(i);
                    ExportSheet(sheet, sw);
                }
                sw.WriteLine("]}");
            }

            Console.WriteLine($"{protoName}导表完成");
        }

        private void ExportSheet(ISheet sheet, StreamWriter sw)
        {
            int cellCount = sheet.GetRow(0).LastCellNum;

            CellInfo[] cellInfos = new CellInfo[cellCount];

            for (int i = 0; i < cellCount; i++)
            {
                string fieldDesc = ExcelHelper.GetCellString(sheet, 0, i);
                string fieldName = ExcelHelper.GetCellString(sheet, 1, i);
                string fieldType = ExcelHelper.GetCellString(sheet, 2, i);
                cellInfos[i] = new CellInfo() { Name = fieldName, Type = fieldType, Desc = fieldDesc };
            }

            for (int i = 3; i <= sheet.LastRowNum; ++i)
            {
                StringBuilder sb = new StringBuilder();
                if (ExcelHelper.GetCellString(sheet, i, 0) == "")
                {
                    
                    break; ;
                }
                if(i>3)
                    sb.Append(",");

                IRow row = sheet.GetRow(i);
                sb.Append("{");
                for (int j = 0; j < cellCount; ++j)
                {

                    string fieldValue = ExcelHelper.GetCellString(row, j);
                    if (fieldValue == "")
                    {
                        continue;
                    }

                    if (j > 0 )
                    {
                        sb.Append(",");
                    }

                    string fieldName = cellInfos[j].Name.ToLower();

                    string fieldType = cellInfos[j].Type;
                    sb.Append($"\"{underscope_field(fieldName)}\":{Convert(fieldType, fieldValue)}");
                }

                sb.Append("}");
          
                
                sw.WriteLine(sb.ToString());
            }
        }

        private static string Convert(string type, string value)
        {
            switch (type)
            {
                case "int[]":
                case "int32[]":
                case "long[]":
                    return $"[{value}]";
                case "string[]":
                    return $"[{value}]";
                case "int":
                case "int32":
                case "int64":
                case "long":
                case "float":
                case "double":
                    return value;
                case "string":
                    return $"\"{value}\"";
                default:
                    throw new Exception($"不支持此类型: {type}");
            }
        }

        private static string GetTSType(string type)
        {
            switch (type)
            {
                case "int[]":
                case "int32[]":
                case "long[]":
                    return "Array<number>";
                case "string[]":
                    return "Array<string>";
                case "int":
                case "int32":
                case "int64":
                case "long":
                case "float":
                case "double":
                    return "number";
                case "string":
                    return "string";
                default:
                    throw new Exception($"不支持此类型: {type}");
            }
        }

        private static string GetTSDefaultValue(string type)
        {
            switch (type)
            {
                case "int[]":
                case "int32[]":
                case "long[]":
                case "string[]":
                    return null;
                case "int":
                case "int32":
                case "int64":
                case "long":
                case "float":
                case "double":
                    return "0";
                case "string":
                    return "";
                default:
                    throw new Exception($"不支持此类型: {type}");
            }
        }

    }
}
