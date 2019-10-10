// this is a demo of melkior current code
// the extension is "lua" to trick the ide to show syntax highlight
// variable definition
func main(args)
    print args;
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
    print dict_1.entry_1;

    // control statements

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

    // ternary operator

    var string_2 = number_1 == number_2
        yep "number_1 equals number_2"
        nop "number_1 is not equal number_2";

    print string_2;

    // foreach loop
    do
        var list = ["Jan", "Feb", "Mar", "Apr"];
        var months = "";
        foreach item with index in  list do
            months += item + ", ";
        end
        print months;
    end

    // array mapping
    do
        var squares = [0,0,0,0,0,0].map(
            lambda(v,k) => k * k
        );
        var list = "";
        squares.each(lambda(v)=>list += v + ", ");
        print list;
    end

    // array mapping
    do
        var squares = [0,0,0,0,0,0].map(
            func(v,k)
                return k * k;
            end
        );
        var list = "";
        squares.each(
            func(v)
                list += v + ", ";
            end
        );
        print list;
    end


    // string methods

    print "alpha".concat("beta");
    print "alpha".contains("beta") +  " = false";
    print "alpha".contains("pha") +  " = true";
    print "alpha".includes("lph") +  " = true";
    print "toupercase".toupper();
    print "TOLOWERCASE".tolower();
    print "a,b,c,d,e,f,g,h".split(",");
    print "the substring should be substring".substring(24);


    class ClassName
        constructor(alpha, gama)
            print "test constructor";
            this.property = alpha + gama;
        end 

        method(alpha, beta)
            print alpha + beta;
        end
    end

    class Child inherits ClassName
    end

    var instance = new Child("first", "secondd");
    instance.method("alpha", "omega");

     class Person
        constructor(name)
            this.name = name;
        end 

        method()
            print this.name + " says hi";
        end
    end

    class Student inherits Person

        constructor(name, grade)
            base.constructor(name);
            this.grade = grade;
        end

        method()
            base.method();
            print "and also is in grade " + this.grade;
        end

    end

    var obj = new Student("Jhon", 22);
    obj.method();


    func sample(a, b)
        return a + b;
    end

    print (sample("hello", "world1")).reverse(); 

    pause
end
