using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogicSpace.Prediction
{
    public static class RequestPrinter
    {
        public static string ToLogString(this IEnumerable<IRequest> requests)
        {
            if (requests == null || !requests.Any())
                return "Request Queue: [Empty]";

            var sb = new StringBuilder();
            sb.AppendLine($"Request Queue ({requests.Count()} items):");

            var index = 1;
            foreach (var req in requests)
            {
                var description = req switch
                {
                    MoveRequest m => $"Move [{m.target}] -> {m.direction}",
                    RotateRequest r => $"Rotate [{r.target}] -> Look {r.lookDirection}",
                    StopRequest s => $"Stop [{s.target}]",
                    _ => $"Unknown Request: {req}"
                };

                sb.AppendLine($"{index++}. {description}");
            }

            return sb.ToString();
        }
    }
}