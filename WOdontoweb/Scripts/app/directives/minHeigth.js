app.directive("minHeigth", function ($window) {
  
        return {
            restrict: 'A',

            link: function (scope, element, attrs) {
              
                var newSize = function () {
                    var h = $(window).height();
                    var rest = $("#headerTotal").height();
                    return h - rest -75;
                };

                $(document).on('onResize', function (e, args) {
                    executeResizeWithTime(300);
                });

                angular.element($window).bind('resize', function () {
                    executeResizeWithTime(300);
                });

                executeResizeWithTime(300);

                function executeResizeWithTime(time) {
                   
                    setTimeout(function () { $(element).css({ minHeight: newSize(), maxHeight: newSize() }) }, time);
                }

            }
        };
    
});

app.directive("maxHeigth", function ($window) {

    return {
        restrict: 'A',

        link: function (scope, element, attrs) {

            var newSize = function () {
                var h = $(window).height();
                var rest = $("#headerTotal").height();
                return h -rest - 115;
            };

            $(document).on('onResize', function (e, args) {
                executeResizeWithTime(300);
            });

            angular.element($window).bind('resize', function () {
                executeResizeWithTime(300);
            });

            executeResizeWithTime(300);

            function executeResizeWithTime(time) {
             
                setTimeout(function () { $(element).css({ 'max-height': newSize()+'px' }) }, time);
            }

        }
    };

});

app.directive("maxHeigthTable", function ($window) {

    return {
        restrict: 'A',

        link: function (scope, element, attrs) {

            var newSize = function () {
                var h = $(window).height();
                var rest = $("#headerTotal").height();
                return h - rest - 220;
            };

            $(document).on('onResize', function (e, args) {
                executeResizeWithTime(300);
            });

            angular.element($window).bind('resize', function () {
                executeResizeWithTime(300);
            });

            executeResizeWithTime(300);

            function executeResizeWithTime(time) {

                setTimeout(function () { $(element).css({ 'max-height': newSize() + 'px' }) }, time);
            }

        }
    };

});
app.directive("maxHeigthModal", function ($window) {

    return {
        restrict: 'A',

        link: function (scope, element, attrs) {

            var newSize = function () {
                var h = $(window).height();
                var rest = $("#headerTotal").height();
                return h - rest - 80;
            };

            $(document).on('onResize', function (e, args) {
                executeResizeWithTime(300);
            });

            angular.element($window).bind('resize', function () {
                executeResizeWithTime(300);
            });

            executeResizeWithTime(300);

            function executeResizeWithTime(time) {

                setTimeout(function () { $(element).css({ 'max-height': newSize() + 'px' }) }, time);
            }

        }
    };

});