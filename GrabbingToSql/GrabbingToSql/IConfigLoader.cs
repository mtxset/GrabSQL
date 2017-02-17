using System.Collections.Generic;

namespace GrabbingToSql
{
    public interface IConfigLoader
    {
        Dictionary<string, string> LoadFields(Parser.PageTab tab = Parser.PageTab.Overview);
    }
}