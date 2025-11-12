namespace ModuWeb.Api.Services
{
    public interface IConversionService
    {
        /// <summary>
        /// Converts JSON string to YAML format
        /// </summary>
        /// <param name="jsonContent">JSON string to convert</param>
        /// <returns>YAML formatted string</returns>
        string ConvertJsonToYaml(string jsonContent);

        /// <summary>
        /// Converts YAML string to JSON format
        /// </summary>
        /// <param name="yamlContent">YAML string to convert</param>
        /// <returns>JSON formatted string</returns>
        string ConvertYamlToJson(string yamlContent);
    }
}