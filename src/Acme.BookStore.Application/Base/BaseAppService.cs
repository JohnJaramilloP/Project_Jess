using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Acme.BookStore.Ratings;
using Acme.BookStore.DTO.Base;
using Acme.BookStore.Base;
using System.Globalization;
using System.IO;
using System.Data;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Users;
using Acme.BookStore.DTO.Ratings;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application.Services;
using Volo.Abp.DependencyInjection;
using Volo.Abp.PermissionManagement;
using Microsoft.Data.SqlClient;
using System.Xml.Linq;
using System.Data.SqlClient;

using Volo.Abp.EntityFrameworkCore;
using Acme.BookStore.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Acme.BookStore.Base
{
    public class BaseAppService : BookStoreAppService, IBaseAppService
    {
        private readonly IRepository<Rating, Guid> _ratingRepository;
        private readonly IDbContextProvider<BookStoreDbContext> _dbContextProvider;

        public BaseAppService(
            IRepository<Rating, Guid> ratingRepository,
            IDbContextProvider<BookStoreDbContext> dbContextProvider
        )
        {
            _ratingRepository = ratingRepository;
            _dbContextProvider = dbContextProvider;
        }

        //Get lista Ratings
        public async Task<List<RatingDto>> GetListRatingsDto()
        {
            var listado = await _ratingRepository.GetQueryableAsync();

            var config = new MapperConfiguration(cfg =>
                cfg.CreateMap<Rating, RatingDto>()
               );

            var mapper = new Mapper(config);

            return mapper.Map<List<RatingDto>>(listado);

        }

        //Crear Ratings
        public async Task<Guid> CrearRating(RatingDto data)
        {
            var config = new MapperConfiguration(cfg =>
                cfg.CreateMap<RatingDto, Rating>()
               );

            var mapper = new Mapper(config);
            var creado = mapper.Map<Rating>(data);

            var respuestaCreada = await _ratingRepository.InsertAsync(creado);

            return respuestaCreada.Id;
        }

        //Editar Ratings
        public async Task editarRating(RatingDto data)
        {
            var rating = await _ratingRepository.GetAsync(data.Id);

            rating.qar = data.qar;
            rating.record_type = data.record_type;
            rating.qa_analyst = data.qa_analyst;
            rating.session_type = data.session_type;
            rating.date = data.date;
            rating.overall_rating = data.overall_rating;
            rating.score_string = data.score_string;
            rating.code = data.code;
            rating.name = data.name;

            await _ratingRepository.UpdateAsync(rating);
        }

        //Eliminar Ratings
        public async Task eliminarRating(Guid Id)
        {
            await _ratingRepository.DeleteAsync(Id);
        }

        public async Task ProcesarIncentivos(string pathFile)
        {
            int estado = 0;
            var dbContext = await _dbContextProvider.GetDbContextAsync();
            try
            {

                List<Rating> ListadoDocumentos = ReadExcelFileIncentivos(pathFile);

                int valorRetornar = 0;

                dbContext.Ratings.AddRange(ListadoDocumentos);


            }
            catch (Exception exx)
            {

            }
        }

        public static string GetCellValue(SpreadsheetDocument document, Cell cell)
        {
            try
            {
                SharedStringTablePart stringTablePart = document.WorkbookPart.SharedStringTablePart;
                if (cell.CellValue == null)
                {
                    return "";
                }
                string value = cell.CellValue.InnerXml;
                if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
                {
                    return stringTablePart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText;
                }
                else
                {
                    return value;
                }
            }
            catch (Exception exx)
            {
                return "";
            }
        }

        public static int? GetColumnIndexFromName(string columnName)
        {

            //return columnIndex;
            string name = columnName;
            int number = 0;
            int pow = 1;
            for (int i = name.Length - 1; i >= 0; i--)
            {
                number += (name[i] - 'A' + 1) * pow;
                pow *= 26;
            }
            return number;
        }

        public static string GetColumnName(string cellReference)
        {
            // Create a regular expression to match the column name portion of the cell name.
            Regex regex = new Regex("[A-Za-z]+");
            Match match = regex.Match(cellReference);
            return match.Value;
        }

        public List<Rating> ReadExcelFileIncentivos(string fileName)
        {
            List<Rating> listadoDocumentos = new List<Rating>();

            string resul = "";
            DataTable dt = new DataTable();

            try
            {
                using (SpreadsheetDocument spreadSheetDocument = SpreadsheetDocument.Open(fileName, false))
                {
                    resul += "STEP A";
                    WorkbookPart workbookPart = spreadSheetDocument.WorkbookPart;
                    resul += "STEP B";
                    IEnumerable<Sheet> sheetsD = spreadSheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>();
                    resul += "STEP C";
                    foreach (Sheet sht in sheetsD)
                    {
                        resul += "NOMBRE HOJA VALIDACION = " + sht.Name;
                        if (true)
                        {

                            resul += "INGRESA A LECTURA";
                            string relationshipId = sht.Id.Value;
                            WorksheetPart worksheetPart = (WorksheetPart)spreadSheetDocument.WorkbookPart.GetPartById(relationshipId);
                            Worksheet workSheet = worksheetPart.Worksheet;
                            SheetData sheetData = workSheet.GetFirstChild<SheetData>();
                            IEnumerable<Row> rows = sheetData.Descendants<Row>();
                            foreach (Cell cell in rows.ElementAt(0))
                            {
                                dt.Columns.Add(GetCellValue(spreadSheetDocument, cell));
                            }
                            foreach (Row row in rows) //this will also include your header row...
                            {
                                DataRow tempRow = dt.NewRow();
                                int columnIndex = 0;
                                foreach (Cell cell in row.Descendants<Cell>())
                                {
                                    // Gets the column index of the cell with data
                                    int cellColumnIndex = (int)GetColumnIndexFromName(GetColumnName(cell.CellReference));
                                    cellColumnIndex--; //zero based index
                                    if (columnIndex < cellColumnIndex)
                                    {
                                        do
                                        {
                                            tempRow[columnIndex] = ""; //Insert blank data here;
                                            columnIndex++;
                                        }
                                        while (columnIndex < cellColumnIndex);
                                    }
                                    tempRow[columnIndex] = GetCellValue(spreadSheetDocument, cell);

                                    columnIndex++;
                                }
                                dt.Rows.Add(tempRow);
                            }

                        }
                    }
                }
                dt.Rows.RemoveAt(0); //...so i'm taking it out here.
            }
            catch (Exception ex)
            {
                resul += "ERROR: " + ex;
            }


            listadoDocumentos = (from DataRow dr in dt.Rows
                                 select new Rating()
                                 {
                                     qar = dr[0]?.ToString() ?? "",
                                     record_type = dr[1]?.ToString() ?? "",
                                     qa_analyst = dr[2]?.ToString() ?? "",
                                     session_type = dr[3]?.ToString() ?? "",
                                     date = ConvertExcelDate(dr[4]?.ToString() ?? ""),
                                     date_time = ConvertExcelDateTime(dr[4]?.ToString() ?? ""),
                                     //date = DateTime.TryParse(dr[4]?.ToString(), out DateTime parsedDate)
                                     //           ? parsedDate
                                     //           : default(DateTime),
                                     overall_rating = dr[5]?.ToString() ?? "",
                                     score_string = dr[6] != DBNull.Value
                                                        ? FormatScore(dr[6]?.ToString())
                                                        : "",
                                     code = dr[7]?.ToString() ?? "",
                                     name = dr[8]?.ToString() ?? "",
                                     Id = Guid.NewGuid(),
                                 }).ToList();




            return listadoDocumentos;
        }

        private static string ConvertExcelDate(string excelDateString)
        {
            if (string.IsNullOrWhiteSpace(excelDateString))
            {
                return string.Empty;
            }

            if (double.TryParse(excelDateString, NumberStyles.Any, CultureInfo.InvariantCulture, out double parsedValue))
            {
                try
                {
                    // Convertir el número OADate a DateTime
                    DateTime dateTime = DateTime.FromOADate(parsedValue);
                    // Formatear la fecha como una cadena
                    return dateTime.ToString("yyyy-MM-dd HH:mm"); // Ajustar el formato según sea necesario
                }
                catch (ArgumentOutOfRangeException)
                {
                    // Manejo específico para valores fuera del rango de DateTime
                    return "Invalid Date";
                }
                catch (Exception ex)
                {
                    // Manejo de cualquier otra excepción
                    return $"Error: {ex.Message}";
                }
            }
            else
            {
                // Manejo si el valor no es un número
                return "Invalid Date Format";
            }
        }

        private static DateTime ConvertExcelDateTime(string excelDateString)
        {
            if (string.IsNullOrWhiteSpace(excelDateString))
            {
                // Retorna la fecha mínima con el tiempo en ceros si el valor está vacío
                return new DateTime(1900, 1, 1);
            }

            if (double.TryParse(excelDateString, NumberStyles.Any, CultureInfo.InvariantCulture, out double parsedValue))
            {
                try
                {
                    // Convertir el número OADate a DateTime
                    DateTime dateTime = DateTime.FromOADate(parsedValue);

                    // Ajustar el DateTime para que tenga horas, minutos y segundos en cero
                    return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
                }
                catch (ArgumentOutOfRangeException)
                {
                    // Manejo específico para valores fuera del rango de DateTime
                    return new DateTime(1900, 1, 1);
                }
                catch (Exception ex)
                {
                    // Manejo de cualquier otra excepción
                    // Puedes registrar el error si lo deseas
                    return new DateTime(1900, 1, 1);
                }
            }
            else
            {
                // Manejo si el valor no es un número
                return new DateTime(1900, 1, 1);
            }
        }


        private static string FormatScore(string scoreString)
        {
            if (decimal.TryParse(scoreString, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal score))
            {
                // Convertir el valor a porcentaje
                decimal percentage = score * 100;

                // Redondear a dos decimales
                decimal roundedPercentage = Math.Round(percentage, 2, MidpointRounding.AwayFromZero);

                // Convertir el valor redondeado a cadena con formato de dos decimales
                string formattedScore = roundedPercentage.ToString("F2", CultureInfo.InvariantCulture);

                return formattedScore;
            }

            // Si no se puede analizar el porcentaje, devolver un valor vacío o manejar el error.
            return "";
        }


        private string ConvertDate(object dateValue)
        {
            if (dateValue == null)
                return "Invalid date";

            string dateString = dateValue.ToString();
            if (double.TryParse(dateString, out double parsedValue))
            {
                return DateTime.FromOADate(parsedValue).ToString("yyyy-MM-dd");
            }
            else if (DateTime.TryParse(dateString, out DateTime parsedDate))
            {
                return parsedDate.ToString("yyyy-MM-dd");
            }

            return "Invalid date";
        }

    }
}
