using StarTrekWebApp.Models;
using System.Text.RegularExpressions;
using static StarTrekWebApp.Common.Enums;

namespace StarTrekWebApp.Editor
{
    public class EditorProcessor
    {
        public EditorProcessor()
        {

        }

        public SpacecraftRequestDto? ProcessFormDictionary(Dictionary<string, string> requestData)
        {
            if (requestData == null)
                return null;

            var errors = new List<ErrorDto>();

            if (requestData.ContainsKey("action"))
            {
                if (requestData["action"] == "create")
                {
                    if (!requestData.ContainsKey("data[0][name]") || string.IsNullOrWhiteSpace(requestData["data[0][name]"]))
                        errors.Add(new ErrorDto { Name = "name", Status = "A name is required" });

                    if (!requestData.ContainsKey("data[0][registry]") || string.IsNullOrWhiteSpace(requestData["data[0][registry]"]))
                        errors.Add(new ErrorDto { Name = "registry", Status = "A registry is required" });

                    return new SpacecraftRequestDto()
                    {
                        Action = RequestActions.CREATE,
                        Spacecraft = new Spacecraft()
                        {
                            Name = requestData.ContainsKey("data[0][name]") ? requestData["data[0][name]"] : string.Empty,
                            Registry = requestData.ContainsKey("data[0][registry]") ? requestData["data[0][registry]"] : string.Empty,
                            SystemDate = DateTime.Now,
                            LastChange = null,
                            Status = requestData.ContainsKey("data[0][status]") ? requestData["data[0][status]"] : string.Empty,
                            DateStatus = requestData.ContainsKey("data[0][dateStatus]") ? requestData["data[0][dateStatus]"] : string.Empty
                        },
                        Errors = errors
                    };
                }
                else if (requestData["action"] == "edit")
                {
                    var results = from result in requestData
                                  where Regex.Match(result.Key, @"data\[(.*?)\]\[(.*?)\]").Success
                                  select result;
                    if(results != null && results.Count() > 0)
                    {
                        var uid = Regex.Match(results.FirstOrDefault().Key, @"data\[(.*?)\]\[(.*?)\]").Groups?[1]?.ToString();
                        if(!string.IsNullOrWhiteSpace(uid))
                        {
                            if (!results.Any(r => r.Key.Contains("name")) || string.IsNullOrWhiteSpace(requestData[$"data[{uid}][name]"]))
                                errors.Add(new ErrorDto { Name = "name", Status = "A name is required" });

                            if (!results.Any(r => r.Key.Contains("registry")) || string.IsNullOrWhiteSpace(requestData[$"data[{uid}][registry]"]))
                                errors.Add(new ErrorDto { Name = "registry", Status = "A registry is required" });

                            return new SpacecraftRequestDto()
                            {
                                Action = RequestActions.UPDATE,
                                Spacecraft = new Spacecraft()
                                {
                                    Uid = uid,
                                    Name = results.Any(r => r.Key.Contains("name")) ? requestData[$"data[{uid}][name]"] : string.Empty,
                                    Registry = results.Any(r => r.Key.Contains("registry")) ? requestData[$"data[{uid}][registry]"] : string.Empty,
                                    LastChange = DateTime.Now,
                                    Status = results.Any(r => r.Key.Contains("status")) ? requestData[$"data[{uid}][status]"] : string.Empty,
                                    DateStatus = results.Any(r => r.Key.Contains("dateStatus")) ? requestData[$"data[{uid}][dateStatus]"] : string.Empty,
                                },
                                Errors = errors
                            };
                        }
                    }
                }
                else if (requestData["action"] == "remove")
                {
                    var results = from result in requestData
                                  where Regex.Match(result.Key, @"data\[(.*?)\]\[uid\]").Success
                                  select result;
                    if (results != null && results.Count() > 0)
                    {
                        var uid = Regex.Match(results.FirstOrDefault().Key, @"data\[(.*?)\]\[(.*?)\]").Groups?[1]?.ToString();

                        return new SpacecraftRequestDto()
                        {
                            Action = RequestActions.DELETE,
                            Spacecraft = new Spacecraft()
                            {
                                Uid = uid
                            },
                            Errors = errors
                        };
                    }
                }
            }

            return null;
        }
    }
}
