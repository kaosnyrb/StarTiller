﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Core;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;

namespace ssf.IO
{
    public class YamlImporter
    {

        public static T getObjectFromFile<T>(string filePath)
        {
            string content = File.ReadAllText(filePath);
            return getObjectFromYaml<T>(content);
        }

        public static T getObjectFromYaml<T>(string yaml)
        {
            var deserializer = new DeserializerBuilder()
            .WithNamingConvention(PascalCaseNamingConvention.Instance)  // see height_in_inches in sample yml 
            .Build();

            //yml contains a string containing your YAML
            T obj = deserializer.Deserialize<T>(yaml);
            return obj;

        }
    }
}
