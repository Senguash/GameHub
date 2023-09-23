using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class UIGenerate
{
    public static VisualElement VisualElement(VisualElement parent, StyleLength width, StyleLength height, FlexDirection flexDirection = FlexDirection.Column, Align align = Align.Auto, Justify justify = Justify.Center, bool colorRandomly = false)
    {
        VisualElement ve = new VisualElement();
        ve.style.width = width;
        ve.style.height = height;
        ve.style.flexDirection = flexDirection;
        ve.style.alignItems = align; //Alignment in cross direction of flex direction
        ve.style.justifyContent = justify; //Alignment in direction of flex direction
        parent.Add(ve);

        if (colorRandomly) //Use to see element in testing
        {
            ve.style.backgroundColor = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
        }

        return ve;
    }
    public static Button Button(VisualElement parent, string text = "")
    {
        Button btn = new Button();
        btn.text = text;
        parent.Add(btn);
        return btn;
    }
    public static Label Label(VisualElement parent, string text, int fontsize = 11)
    {
        Label lbl = new Label();
        lbl.text = text;
        lbl.style.fontSize = fontsize;
        parent.Add(lbl);
        return lbl;
    }
    public static ScrollView ScrollView(VisualElement parent)
    {
        ScrollView sv = new ScrollView();
        sv.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
        sv.verticalScrollerVisibility = ScrollerVisibility.Hidden;
        parent.Add(sv);
        return sv;
    }
}
