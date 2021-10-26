let a="",b="",c="";let d=true;
for(i=2;i<process.argv.length;i++)
{
    a+=process.argv[i];if(i!==process.argv.length-1) a+=" ";
}
let e=a.split(' ');
if(e.length>1)
{
    for(let i=0;i<e[0].length;i++)
    {
        d=true;b+=e[0][i];
        for(j=1;j<e.length;j++)
        {
            if(!e[j].includes(b))
            {
                i=i-(b.length-1);b="";d=false;
            }
        }
        if(b.length>c.length&&d===true) c=b;
    } 
}
console.log(c);