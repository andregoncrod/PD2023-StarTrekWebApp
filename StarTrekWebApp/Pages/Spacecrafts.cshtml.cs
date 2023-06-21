using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Win32;
using StarTrekWebApp.Common;
using StarTrekWebApp.Editor;
using StarTrekWebApp.Models;
using System;
using System.Data;
using System.Text.Json;
using System.Xml.Linq;
using static StarTrekWebApp.Common.Enums;

namespace StarTrekWebApp.Pages
{
    [IgnoreAntiforgeryToken]
    public class SpacecraftsModel : PageModel
    {
        private EditorProcessor editorProcessor;
        private HttpHelper httpHelper;

        public SpacecraftsModel()
        {
            this.editorProcessor = new EditorProcessor();
            this.httpHelper = new HttpHelper();
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnGetTableData()
        {
            int draw = int.Parse(Request.Query["draw"]);
            int start = int.Parse(Request.Query["start"]);
            int length = int.Parse(Request.Query["length"]);
            string searchValue = Request.Query["search[value]"];
            int orderColumn = int.Parse(Request.Query["order[0][column]"]);
            string orderDir = Request.Query["order[0][dir]"];
            int page = (start / length) + 1;

            var spacecraftsResp = await httpHelper.Get($"{AppSettings.APIUrl}/Spacecraft/paged/{page}/{length}/{GetColumnNameByNumber(orderColumn)}/{orderDir}/{searchValue}");

            SpacecraftPagedJson spacecraftsPaged = JsonSerializer.Deserialize<SpacecraftPagedJson>(spacecraftsResp);

            // Create the response object
            var response = new
            {
                draw,
                recordsTotal = spacecraftsPaged?.total,
                recordsFiltered = spacecraftsPaged?.filtered,
                data = spacecraftsPaged?.results?.Select(s => new
                {
                    s.uid,
                    s.name,
                    s.registry,
                    s.dateStatus,
                    s.lastChange,
                    s.status,
                    s.systemDate
                })
            };

            return new JsonResult(response);            
        }

        public async Task<IActionResult> OnPostTableDataAsync()
        {
            var requestData = Request.Form.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.ToString()
            );

            var request = editorProcessor.ProcessFormDictionary(requestData);
            
            object? response =  new
            {
                success = false
            };
            if (request != null)
            {
                if (request.Errors.Count > 0)
                {
                    response = new
                    {
                        fieldErrors = request.Errors.Select(e => new
                        {
                            name = e.Name,
                            status = e.Status
                        }),
                        data = new { },
                        success = false
                    };
                }
                else
                {
                    bool success = await ExecuteActions(request);

                    if (success)
                    {
                        var list = new List<Spacecraft>();
                        list.Add(request.Spacecraft);
                        response = new
                        {
                            success = true,
                            data = list.Select(s => new
                            {
                                name = s.Name
                            })
                        };
                    }
                }
            }

            return new JsonResult(response);
        }

        private string GetColumnNameByNumber(int columnNr)
        {
            string columnName = string.Empty;
            switch (columnNr)
            {
                case 0:
                    columnName = "Name";
                    break;
                case 1:
                    columnName = "Registry";
                    break;
                case 2:
                    columnName = "Status";
                    break;
                case 3:
                    columnName = "DateStatus";
                    break;
                case 4:
                    columnName = "SystemDate";
                    break;
                case 5:
                    columnName = "LastChange";
                    break;
                default:
                    break;
            }
            return columnName;
        }

        private async Task<bool> ExecuteActions(SpacecraftRequestDto requestDto)
        {
            if(requestDto.Action == RequestActions.CREATE)
            {
                var addRsp = await httpHelper.Post($"{AppSettings.APIUrl}/Spacecraft", JsonSerializer.Serialize(new
                {
                    name = requestDto.Spacecraft.Name,
                    registry = requestDto.Spacecraft.Registry,
                    status = requestDto.Spacecraft.Status,
                    dateStatus = requestDto.Spacecraft.DateStatus,
                }));
                return addRsp != null ? bool.Parse(addRsp) : false;
            }
            else if(requestDto.Action == RequestActions.UPDATE)
            {
                var updRsp = await httpHelper.Put($"{AppSettings.APIUrl}/Spacecraft/{requestDto.Spacecraft.Uid}", JsonSerializer.Serialize(new
                {
                    name = requestDto.Spacecraft.Name,
                    registry = requestDto.Spacecraft.Registry,
                    status = requestDto.Spacecraft.Status,
                    dateStatus = requestDto.Spacecraft.DateStatus,
                }));
                return updRsp != null && bool.Parse(updRsp);
            }
            else if(requestDto.Action == RequestActions.DELETE)
            {
                var deleteRsp = await httpHelper.Delete($"{AppSettings.APIUrl}/Spacecraft/{requestDto.Spacecraft.Uid}");
                return deleteRsp != null ? bool.Parse(deleteRsp) : false;
            }
            return false;
        }
    }
}
