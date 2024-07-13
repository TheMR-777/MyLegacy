class my_type
{
	public int x, y;

    public my_type(int x)
    {
        this.x = x;
        y = 0;
    }

    public my_type(int x, int y)
        : this(x)
    {
        this.y = y;
    }
}