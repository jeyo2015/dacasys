app.filter('filterByWords', function () {
    return function myFilterByWords(array, substr, fieldNames, expressions, isAcceptingTheSpecifiedFields, recurseField, oneIsEnough) {
        if (!angular.isArray(array) || (!substr && substr !== 0))
            return array;

        function containsWord(text, word) {
            return text.toUpperCase().indexOf(word.toUpperCase()) >= 0;
        }

        function isAnAcceptedField(field) {
            var position = jQuery.inArray(field, fieldNamesList);
            return isAcceptingTheSpecifiedFields ? position >= 0 : position < 0;
        }

        function expression(theObject, field) {
            var text = expressions && expressions[field] ? expressions[field](theObject, field) : theObject[field];
            return (!text && text !== 0) ? '' : '' + text;
        }

        function hasAMatchingChild(theObject) {
            var list = [],
                hasChildren = recurseField && (list = theObject[recurseField]) && list.length;
            if (!hasChildren)
                return false;
            list = myFilterByWords(list, substr, fieldNames, expressions, isAcceptingTheSpecifiedFields, recurseField, true);
            return list.length > 0;
        }

        function matches(theObject) {
            if (hasAMatchingChild(theObject))
                return true;
            var remaining = words.length,
                theWords = angular.copy(words);
            for (var field in theObject)
                if (isAnAcceptedField(field))
                    for (var position = 0; position < remaining;)
                        if (!containsWord(expression(theObject, field), theWords[position]))
                            position++;
                        else {
                            theWords.splice(position, 1);
                            remaining--;
                            if (remaining == 0)
                                return true;
                        }
            return false;
        }

        function myTrim(str) {
            return (typeof str == "string" ? str : '').replace(/^[,\s]+|[,\s]+$/g, '');
        }

        if (typeof substr != "string")
            substr = typeof substr == "number" ? '' + substr : '';
        fieldNames = myTrim(fieldNames);
        var fieldNamesList = /[^ ,]/.test(fieldNames) ? fieldNames.split(/[ ,]+/) : [];
        if (!isAcceptingTheSpecifiedFields)
            fieldNamesList.push("$$hashKey");
        var result = [],
            words = substr.trim().split(/ +/);
        if (words.length == 1 && !words[0])
            return array;
        for (var index = 0, limit = array.length; index < limit; index++)
            if (matches(array[index])) {
                result.push(array[index]);
                if (oneIsEnough)
                    break;
            }
        return result;
    };
});

app.filter('filterUsers', ['$filter', function ($filter) {
    return function (array, substr) {
        return $filter('filterByWords')(array, substr,
            'PkId,IsDisabled,IsChecked,IsActiveManagementUnit,FkManagementUnit',
            {
                'Surname': function (theObject, field) {
                    return theObject[field] + ',';
                }
            }
        );
    };
}]);

app.filter('filterWellbores', ['$filter', function ($filter) {
    return function (array, substr, pkProductList) {
        if (!array) return [];
        var filteredList = pkProductList.length?
            array.where(function (wellbore) {
                var pkWellboreProductList = wellbore.ProductIds.split("|");
                var selectedProduct = null;
                for (var i = 0; i < pkProductList.length; i++) {
                    selectedProduct = pkProductList[i];
                    if (pkWellboreProductList.indexOf(String(selectedProduct)) >-1)
                        return true;
                }
                return false;
            }):
        [];
        
        return $filter('filterByWords')(filteredList, substr,
            'PkWellbore,FkManagementUnit',
            {
                'WellboreType': function (theObject, field) {
                    return theObject[field].Name;
                }
            }
        );
    };
}]);

app.filter('filterByFields', ['$filter', function ($filter) {
    return function (array, substr, acceptedFields, expressions) {
        return $filter('filterByWords')(array, substr, acceptedFields, expressions, true);
    };
}]);


app.filter('filterProjectVersionForPlanning', ['$filter', function ($filter) {
    return function (array, substr) {
        return $filter('filterByWords')(array, substr, 'ProjectName,ProjectVersionLabel,ManagementUnitPath,Responsible,ModifiedBy', 0 ,1, 'AnalysisScenarios');
    };
}]);


app.filter('orderByScenario', function () {

    function orderScenarioBy(scenario) {
        if (scenario.Version == 0) {
            return scenario.Scenario;
        }
        return scenario.Scenario == 1 ? scenario.Pk * 2 : scenario.Pk;
    }

    return function (scenario) {
        var array = [];
        if (!scenario)
            return array;
        Object.keys(scenario).forEach(function (key) {
            scenario[key].name = key;
            array.push(scenario[key]);
        });
        array.sort(function (a, b) {
            if (a.Version == 0) {
                return orderScenarioBy(a) - orderScenarioBy(b);
            } else {
                return orderScenarioBy(b) - orderScenarioBy(a);
            }
        });
        return array;
    };
});

app.filter('orderByScenario', function () {

    function orderScenarioBy(scenario) {
        if (scenario.Version == 0) {
            return scenario.Scenario;
        }
        return scenario.Scenario == 1 ? scenario.Pk * 10 : scenario.Pk;;
    }

    return function (scenario) {
        var array = [];
        if (!scenario)
            return array;
        Object.keys(scenario).forEach(function (key) {
            scenario[key].name = key;
            array.push(scenario[key]);
        });
        array.sort(function (a, b) {
            if (a.Version == 0) {
                return orderScenarioBy(a) - orderScenarioBy(b);
            } else {
                return orderScenarioBy(b) - orderScenarioBy(a);
            }
        });
        return array;
    };
});

app.filter('filterDeleted', function () {
    return function (items, expression) {
        var listFiltered = [];

        angular.forEach(items, function (value) {
            if (value.State !== 3) {
                listFiltered.push(value);
            }
        });
        return listFiltered;
    };
});
app.filter('filterDeletedBool', function () {
    return function (items, expression) {
        var listFiltered = [];

        angular.forEach(items, function (value) {
            if (!value.Eliminar) {
                listFiltered.push(value);
            }
        });
        return listFiltered;
    };
});