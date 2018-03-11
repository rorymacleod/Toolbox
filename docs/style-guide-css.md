# CSS Style Guide

Use SASS files to generate a bundles and minified CSS file.

Separation of concerns: The CSS should contain element resets, reusable components, and a library of reusable utility classes for common padding, margins, borders, backgrounds, etc. The CSS should **not** contain styles that are specific to a particular page's layout, or styles that adjust *this* div, within *this* header, on *this* page. Those specific adjustments are made by applying the required utility classes to the HTML elements, for example:

    <div class="box mar-20 pad-10 pad-lr-30 float-left">
        This element uses a common "box" class that might set background, color, and border, then
        a series of utility classes that adjust the margin, padding, and float.
    </div>     

This advice might appear to contradict the conventional wisdom, which holds that, to achieve "separation of concerns", all styling should appear in the CSS, but in the long-term, that approach leads to several problems:
 
* Every variation of an element needs its own style, bloating the CSS.
* Every variation of an element therefore needs its own class name, or else CSS selectors have to be made increasing specific to pick out a particular element.
* The CSS contains many one-off styles, reducing the style sheet's reusability across pages or projects.
* The CSS evolves to include many cascading styles with subtle interactions, leading to styling bugs.
* Over time, the CSS becomes increasingly difficult to maintain. Styles that might have become redundant are left in for fear of breaking something.
* CSS is tightly bound to the markup.  

## References

* [Challenging CSS Best Practices](https://coding.smashingmagazine.com/2013/10/challenging-css-best-practices-atomic-approach/ "Challenging CSS Best Practices")