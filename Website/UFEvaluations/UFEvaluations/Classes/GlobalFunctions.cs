using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

public static class GlobalFunctions
{
    public static string escapeQuerystringElement(string QuerystringElement)
    {
        Regex reg = new Regex("[?&' ]"); //Pattern to find question marks, ampersands, single quotations, and spaces
        string filteredString = reg.Replace(QuerystringElement, "").ToLower(); //Filter all these characters and return lower-case string

        return filteredString;
    }

    public static string createChartScript(InstructorGraph data)
    {
        return @"var ctx = document.getElementById('instructorChart');
        var instructorChart = new Chart(ctx, {
        type: 'line',
        data:
        {
            labels: " + data.labels + @",
            datasets: [{
                label: 'Ratings',
                data: " + data.data + @",
                borderWidth: 1,
                backgroundColor: 'rgb(17, 102, 167, 0.5)',
                borderColor: 'rgb(0, 85, 150)'
            }]
        },
        options:
        {
            scales:
            {
                yAxes: [{
                    ticks:
                    {
                        beginAtZero: true
                    }
                }]
            }
        }
    });";
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