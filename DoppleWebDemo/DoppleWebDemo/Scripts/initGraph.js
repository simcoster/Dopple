function init() {
    initASingleGraph("myDiagramDiv1", nodeDataArray1, edgeDataArray1);
    initASingleGraph("myDiagramDiv2", nodeDataArray2, edgeDataArray2);

}

function initASingleGraph(divName, nodeDataArray, edgeDataArray) {
    var $ = go.GraphObject.make;  // for conciseness in defining templates
    myDiagram =
    $(go.Diagram, divName,  // create a Diagram for the DIV HTML element
      {
          // position the graph in the middle of the diagram
          initialContentAlignment: go.Spot.Center,
          // enable undo & redo
          "undoManager.isEnabled": true,
          allowLink: false,
          initialAutoScale: go.Diagram.Uniform,
          layout: $(go.LayeredDigraphLayout)
      });
    // Define the appearance and behavior for Nodes:
    // First, define the shared context menu for all Nodes, Links, and Groups.
    // To simplify this code we define a function for creating a context menu button:
    function makeButton(text, action, visiblePredicate) {
        return $("ContextMenuButton",
                 $(go.TextBlock, text),
                 { click: action },
                 // don't bother with binding GraphObject.visible if there's no predicate
                 visiblePredicate ? new go.Binding("visible", "", function (o, e) { return o.diagram ? visiblePredicate(o, e) : false; }).ofObject() : {});
    }
    // a context menu is an Adornment with a bunch of buttons in them
    var partContextMenu =
      $(go.Adornment, "Vertical",
          makeButton("Properties",
                     function (e, obj) {  // OBJ is this Button
                         var contextmenu = obj.part;  // the Button is in the context menu Adornment
                         var part = contextmenu.adornedPart;  // the adornedPart is the Part that the context menu adorns
                         // now can do something with PART, or with its data, or with the Adornment (the context menu)
                         if (part instanceof go.Link) alert(linkInfo(part.data));
                         else if (part instanceof go.Group) alert(groupInfo(contextmenu));
                         else alert(nodeInfo(part.data));
                     }),
          makeButton("Undo",
                     function (e, obj) { e.diagram.commandHandler.undo(); },
                     function (o) { return o.diagram.commandHandler.canUndo(); }),
          makeButton("Redo",
                     function (e, obj) { e.diagram.commandHandler.redo(); },
                     function (o) { return o.diagram.commandHandler.canRedo(); })
      );
    function nodeInfo(d) {  // Tooltip info for a node data object
        var str = "Node " + d.key + " Method: " + d.method + "\n";
        return str;
    }
    // These nodes have text surrounded by a rounded rectangle
    // whose fill color is bound to the node data.
    // The user can drag a node by dragging its TextBlock label.
    // Dragging from the Shape will start drawing a new link.
    myDiagram.nodeTemplate =
      $(go.Node, "Auto",
        { locationSpot: go.Spot.Center },
        $(go.Shape, "RoundedRectangle",
          {
              fill: "white", // the default fill, if there is no data bound value
              portId: "", cursor: "pointer",  // the Shape is the port, not the whole Node
              // allow all kinds of links from and to this port
              fromLinkable: false, fromLinkableSelfNode: false, fromLinkableDuplicates: false,
              toLinkable: false, toLinkableSelfNode: false, toLinkableDuplicates: false
          },
          new go.Binding("fill", "color")),
        $(go.TextBlock,
          {
              font: "bold 27px sans-serif",
              stroke: '#333',
              margin: 6,  // make some extra space for the shape around the text
              isMultiline: true,  // don't allow newlines in text
              editable: false  // allow in-place editing by user
          },
          new go.Binding("text", "text").makeTwoWay()),  // the label shows the node data's text
        { // this tooltip Adornment is shared by all nodes
            toolTip:
              $(go.Adornment, "Auto",
                $(go.Shape, { fill: "#FFFFCC" }),
                $(go.TextBlock, { margin: 4 },  // the tooltip shows the result of calling nodeInfo(data)
                  new go.Binding("text", "", nodeInfo))
              ),
            // this context menu Adornment is shared by all nodes
            contextMenu: partContextMenu
        }
      );
    // Define the appearance and behavior for Links:
    function linkInfo(d) {  // Tooltip info for a link data object
        return "Link:\nfrom " + d.from + " to " + d.to;
    }

    function getDashedArray(d) {
        if (d.type == 1) {
            return [10,5];
        }
        else {
            return [];
        }
    }

    function isAffectingLayout(d) {
        if (d.type == 1) {
            return false;
        }
        else {
            return true;
        }
    }

    // The link shape and arrowhead have their stroke brush data bound to the "color" property
    myDiagram.linkTemplate =
      $(go.Link,
        { toShortLength: 3, relinkableFrom: false, relinkableTo: false },  // allow the user to relink existing links
        $(go.Shape,
          {
              strokeWidth: 4,
          },
          new go.Binding("stroke", "color"),
          new go.Binding("strokeDashArray", "", getDashedArray)),
        $(go.Shape,
          { toArrow: "Standard", stroke: null },
          new go.Binding("fill", "color"),
          new go.Binding("isLayoutPositioned", "" , isAffectingLayout)),
        { // this tooltip Adornment is shared by all links
            toolTip:
              $(go.Adornment, "Auto",
                $(go.Shape, { fill: "#FFFFCC" }),
                $(go.TextBlock, { margin: 4 },  // the tooltip shows the result of calling linkInfo(data)
                  new go.Binding("text", "", linkInfo))
              ),
            // the same context menu Adornment is shared by all links
            contextMenu: partContextMenu
        }
      );

    // Define the behavior for the Diagram background:
    function diagramInfo(model) {  // Tooltip info for the diagram's model
        return "Model:\n" + model.nodeDataArray.length + " nodes, " + model.linkDataArray.length + " links";
    }
    // provide a tooltip for the background of the Diagram, when not over any Part
    myDiagram.toolTip =
      $(go.Adornment, "Auto",
        $(go.Shape, { fill: "#FFFFCC" }),
        $(go.TextBlock, { margin: 4 },
          new go.Binding("text", "", diagramInfo))
      );
    // provide a context menu for the background of the Diagram, when not over any Part
    myDiagram.contextMenu =
      $(go.Adornment, "Vertical",
          makeButton("Undo",
                     function (e, obj) { e.diagram.commandHandler.undo(); },
                     function (o) { return o.diagram.commandHandler.canUndo(); }),
          makeButton("Redo",
                     function (e, obj) { e.diagram.commandHandler.redo(); },
                     function (o) { return o.diagram.commandHandler.canRedo(); })
      );
    myDiagram.model = new go.GraphLinksModel(nodeDataArray, edgeDataArray);
}