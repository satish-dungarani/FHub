$(document).ready(function($) {
    "use strict";
    
    var container_01 = $("#container-01"),
    container_02 = $("#container-02");
    
    container_01.imagesLoaded(function () {
        container_01.pinto({
            itemWidth:200,
            gapX:30,
            gapY:30,
        });
    });
    
    container_02.imagesLoaded(function () {
        container_02.pinto({
            itemWidth:150,
            gapX:15,
            gapY:15,
        });
    });
    
    $("#init").click(function(){
        container_01.pinto();
        container_02.pinto();
    });
    
    $("#destroy").click(function(){
        container_01.pinto("destroy");
        container_02.pinto("destroy");
    });
});