
##  Melkior interpreter
This is a work in progress of an interpreter writen in c# of a scripting language somewhat related to lua/python family.
This project also includes a transpiler of melkior language into javavscript.

### variable definition
```
// numbers
var number_1 = 0.00001;
var number_2 = 9999999;

// string
var string_1 = "string";

// boolean
var boolean_1  = true;
var boolean_2 = false;

// array
var array_1 = [1, 2, 3, 4, 5];
var array_nested = ["one", "two", "three", ["nested_1", "nested_2", array_1]];
print array_nested[3];

// dictionary
var dict_1 = {
    entry_1: "first entry",
    entry_2: "second entry"
};
```
### control statements
```
if number_1 == number_2 then
    print "number_1 equals number_2";
else
    print "numbe_1 is not equal number_2";
end

// mutliple "else if": elseif

if number_1 == number_2 then
    print "number_1 equals number_2";
elseif number_1 == 0.00001 then
    print "number_1 equals 0.0001";
else
    print "number_1 is not 0.0001 nor number_2";
end

// logical operators: and, or , not

if number_1 >= 0 and number_1 < 100 then
    print "number_1 is between 0 and 100";
end
```

### ternary operator
```
condition
    yep "true expression"
    nop "false expression";

var variable_2 = variable_1 == true
    yep "variable_2 equals variable_1"
    nop "variable_2 does not equal variable_1";
```

### block statement
Inside each new block a closure is generated
```
var alpha = "global";
var beta = "global"";
do
    var beta = "local";
    print alpha; // global
    print beta;  // local
end
```
### Loop statements

#### while
```
while condition do
    // statements
end
```
#### repeat while  (do while)
```
repeat
    // statement;
    // statement;
while condition
```

### foreach 
```
foreach item in array_1 do
    print item;
end
```

foreach with index/key

```
foreach item with index in array_1 do
    print index + ': ' + item;
end
```


 ### functions
 In melkior the body of the function is a block statement which must finish with "end"
```
func function_1(alpha, beta)
    var result = alpha + beta;
    return result;
end
```

#### Anonymous functions
When passing a function as argument to another function its possible to use a nameless function
```
["green", "blue", "red"].each(
    func(value, index, array) do
        print index + ": " + value;
    end
);
```

#### Lambda functions
Lambda functions are anonymous functions with a single shorthand return statement in their body

```
var square = [0, 1, 2, 3].map(
    lambda(num) => num * num
);
```

The previous snippet is equivalent to

```
var square = [0, 1, 2, 3].map(
    function(num) do
        return num * num
    end
);
```

### semicolons
Semicolons are required after each
- expression statements
- var statement
- print statement

### classes
Classes are defined by using the reserved keyword "class" folowed by a class name.
To inherit a class from another use "inherits" keyword after class name definition.
Class body consist of function definitions;
To create a new instance use "new" keyword folowed by the Class name and arugment list to call the constructor function.
```
class ParentClass
    
    constructor(alpha) 
        this.alpha = alpha;
    end

    method()
        print this.apha;
    end

end

class ChildClass inherits ParentClass
    
    constructor(alpha, beta)
        base.constructor(alpha); 
        this.beta = beta;
    end

    method()
        base.method();
        print this.beta;
    end

    static(alpha, beta)
        return alpha + beta;
    end

end

var parentInstance = new ParentClass("alpha");
var childInstance = new ChildClass("beta", "gama");
print ChildClass.static("one", "two");
```


## running the interpreter
Execute Melkior from the console passing the source code filename as argument.
Melkior accepts any file extension, currently there is a small demo in the root folder of the project `DemoCode.mel`
```
Melkior.exe source_code.mel
Melkior.exe source_code.mel
```

### passing arguments from command line
It is possible to pass arugments from command line, just append them after the source code filename.
If a `main` function is defined in the source code, Melkior will execute the funcion and pass the command line argumetns as arguments
```
Melkior.exe source.mel argument_1 argument_2 argument_3
```
And in the source code
```
func main(args)
    args[0] == "argument_1";
    args[1] == "argument_2";
end
```

## transpiling to Javascript
It is possible to transpile melkior source code into javascript. It can be done through the cli with `--t` argument.
```
Melkior.exe source.mel --t
```
Melkior will create a new sourcefile with javascript syntaxis by apending `.js` to the orignal filename
