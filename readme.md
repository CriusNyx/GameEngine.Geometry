**GameEngine Geometry**

This library was created as a learning tool, allowing me to learn object oriented design and algorithm design.

This library is far from optimized, or preformant. However, it provides a number of features and algorithms, and was created by analyzing problems I frequently ran into while implementing game engines, and provides a competent solution to them. Additionally, it is very flexible, and easy to implement in many projects.

The algorithms and technologies all work correctly, and although none of them are the most optimal algorithms in existence, an effort was made to make them as optimal as I was able to when I created them by applying when I learned in collage, and my skill as a software engineer and mathematician.

Additionally, this library does show competent use of Object Oriented Design, proper implementation and encapsulation of interfaces and classes, as well as leveraging many of the features of the libraries host language, C#, to allow for some dynamic behavior and transparent micro optimization unique to the language. For example, many simple algorithms which are identical among all polygons are implemented using extension methods, and when a specific data structure is able to provide a more optimal solution, it's able to implement the extension method on it's own, overriding the default behavior.