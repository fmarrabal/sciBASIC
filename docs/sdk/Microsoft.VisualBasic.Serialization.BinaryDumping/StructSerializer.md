﻿# StructSerializer
_namespace: [Microsoft.VisualBasic.Serialization.BinaryDumping](./index.md)_

Some times these method is not works well, not sure why?

> 
>  http://blog.csdn.net/zztoll/article/details/8695992
>  
>  Marshal类的两个方法StructureToPtr和PtrToStructure实现序列化
>  
>  我们主要是使用Marshal类里的两个方法：
>  
>  第一个是StructureToPtr，将数据从托管对象封送到非托管内存块。
>  第二个是PtrToStructure，将数据从非托管内存块封送到新分配的指定类型的托管对象。
>  
>  只要有了这两个相互转换的方法，我们就可以实现序列化了。
>  
>  首先我们先来看下序列化
>  
>  序列化：
>  
>  有一个前提条件，那就是我们必须要知道需要序列化对象的大小。
>  
>  第一步：我们先求出对象的大小，然后在非托管内存中给它分配相应的内存大小。
>  第二步：接着我们就把这个对象封送到刚分配出来的内存中，之后我们会得到一个分配出来的内存块首地址指针。
>  第三步：最后我们可以通过这个首地址指针，从指针所指的位置处开始，拷贝数据到指定的byte[]数组中，
>  拷贝的长度就是我们为这个对象分配出来的内存大小，得到byte[]数据后，下面的事情我就不用多说了，
>  你可以保存到数据库或者文件中。
>  
>  反序列化：
>  
>  序列化的时候我们先将一个对象封送到了非托管内存块中，然后再把内存块中的数据读到byte[]数组中，
>  
>  现在我们反序列化
>  
>  第一步：我们先求出对象的大小，然后在非托管内存中给它分配相应的内存大小。
>  第二步：然后把这个byte[]数据拷贝到非托管内存块中。
>  第三步：最后再从内存块中封送指定大小的数据到对象中。
>  
>  有一个地方需要注意，那就是因为引用类型的对象我们是无法求的它的实际大小的，所以这里的对象我们只能使用非托管对象，比如struct结构体。
>  所以，当我们只是用来存储数据，不涉及任何操作的对象，我们可以把它作为一个结构体来处理，这样我们在序列化的时候可以节省空间开销。
>  因为你如果你要是用平常的序列化方法去序列化一个类对象，他所需要的空间开销是要大于你去序列化一个具有相同结构的struct对象。
>  


### Methods

#### ByteToStructure``1
```csharp
Microsoft.VisualBasic.Serialization.BinaryDumping.StructSerializer.ByteToStructure``1(System.Byte[])
```
由byte数组转换为结构体(字符串类型以及Class类型都将会被序列化为内存指针，所以这个函数只适合于值类型的)

#### StructureToByte``1
```csharp
Microsoft.VisualBasic.Serialization.BinaryDumping.StructSerializer.StructureToByte``1(``0)
```
由结构体转换为byte数组(字符串类型以及Class类型都将会被序列化为内存指针，所以这个函数只适合于值类型的)


