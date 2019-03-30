
##  Melkior interpreter
This is a very early work in progress of an interpreter writen in c# of a scripting language somewhat related to lua/python family.

### variable definition
```
// numbers
var number_1 = 0.00001
var number_2 = 9999999

// string
var string_1 = "string"

// boolean
var boolean_1  = true
var boolean_2 = false

// array
var array_1 = [1, 2, 3, 4, 5]
var array_nested = ["one", "two", "three", ["nested_1", "nested_2", array_1]]
print array_nested[3]

// dictionary
var dict_1 = {
    entry_1: "first entry",
    entry_2: "second entry"
}
```
### control statements
```
if number_1 == number_2 then
    print "number_1 equals number_2"
else
    print "numbe_1 is not equal number_2"
end

// mutliple "else if": elseif

if number_1 == number_2 then
    print "number_1 equals number_2"
elseif number_1 == 0.00001 then
    print "number_1 equals 0.0001"
else
    print "number_1 is not 0.0001 nor number_2"
end

// logical operators: and, or , not

if number_1 >= 0 and number_1 < 100 then
    print "number_1 is between 0 and 100"
end
```

### ternary operator
```
condition
    yep "true expression"
    nop "false expression"

var variable_2 = variable_1 == true
    yep "variable_2 equals variable_1"
    nop "variable_2 does not equal variable_1"
```

### block statement
Inside each new block a closure is generated
```
var alpha = "global"
var beta = "global""
do
    var beta = "local"
    print alpha // global
    print beta  // local
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
    // statement
    // statement
while condition
```

### foreach 
```
foreach item in array_1 do
    print item
end
```

foreach with index/key

```
foreach item with index in array_1 do
    print index + ': ' + item
end
```


 ### functions
 In melkior the body of the function must be a statement.
 But it does not have to be a block statement.

```
func function_1(alpha, beta) do
    var result = alpha + beta;
    return result
end

func function_2(alpha, beta)
    return alpha + beta

func function_2(alpha, beta)
    print alpha + beta
```

#### Anonymous functions
When passing a function as argument to another function its possible to use a nameless function
```
["green", "blue", "red"].each(
    function(value, index, array) do
        print index + ": " + value
    end
)
```

#### Lambda functions
Lambda functions are anonymous functions with a single shorthand return statement in their body

```
var square = [0, 1, 2, 3].map(
    lambda(num): num * num
)
```

The previous snippet is equivalent to

```
var square = [0, 1, 2, 3].map(
    function(num) do
        return num * num
    end
)
```

### running the interpreter

Execute Melkior from the console passing the source code filename as argument.
Melkior accepts any file extension, currently there is a small demo in the root folder of the project `DemoCode.mel`


#### thanks and kudos
to @munificent for publishing the book on [lox interpreter](http://www.craftinginterpreters.com/)
