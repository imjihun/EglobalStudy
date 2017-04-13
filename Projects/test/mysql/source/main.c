

int main()
{
    dbOpen();

    deleteAllTable();
    createAllTable();

    dbClose();
    return 0;
}
