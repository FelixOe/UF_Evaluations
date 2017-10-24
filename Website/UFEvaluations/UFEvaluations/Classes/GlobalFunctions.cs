using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

public static class GlobalFunctions
{
    public static string escapeQuerystringElement(string QuerystringElement)
    {
        Regex reg = new Regex("[?&', ]"); //Pattern to find question marks, ampersands, single quotations, and spaces
        string filteredString = reg.Replace(QuerystringElement, "").ToLower(); //Filter all these characters and return lower-case string

        return filteredString;
    }

    public static string createMultipleChartScript(InstructorGraph data, List<string> labels)
    {
        string script = "";

        script += @"var ctx = document.getElementById('instructorChart');
        if($(window).width() < 400)
            ctx.height = 700;
        else if($(window).width() < 450)
            ctx.height = 650;
        else if($(window).width() < 600)
            ctx.height = 450;
        else if($(window).width() < 767)
            ctx.height = 350;
        var instructorChart = new Chart(ctx, {
        type: 'line',
        data:
        {
            labels: " + data.labels + @",
            datasets: [";

        for(int i = 0; i < data.data.Count(); i++)
        {
            script += @"{
                label: '" + labels[i] + @"',
                data: " + data.data[i] + @",
                borderWidth: 1,
                borderColor: '" + StaticData.graphColors[i] + @"',
                hidden: " + (i == data.data.Count() - 1 || (data.data.Count() > 1 && i == 0) ? "false" : "true") + @"
            },";
        }

        //Delete last comma
        script = script.Remove(script.Length - 1);

        script += @"]
        },
        options:
            {
                scales: {
                    yAxes: [{
                        ticks: {
                            suggestedMin: 1
                        }
                    }]
                },
                responsive: true
            }
        });";

        return script;
    }

    public static string createAutoCompleteScript()
    {
        return @"$('#searchInput').autocomplete({  
                   source: function(request, response) {  
                       $.ajax({
                        url: '/Search/AutoCompleteSearch',  
                           type: 'POST',  
                           dataType: 'json',  
                           data: { term: request.term },  
                           success: function(data) {
                            response($.map(data, function(item) {
                                return { label: item, value: item };
                            }))  
  
                           }
                    })  
                   },  
                   messages:
                {
                    noResults: '', results: ''
                   }
            });";
    }
}