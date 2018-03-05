#include <stdio.h>
#include <stdlib.h>
#include <Windows.h>

#include <time.h>
#include <sys/types.h>
#include <sys/stat.h>
#include <errno.h>

#define _CRT_SECURE_NO_WARNINGS

#define SIZE_BUFFER 4096
#define stat _stat64
#define PATH_SEPARATOR "\\"

#define S_ISREG(m) (((m) & S_IFMT) == S_IFREG)  // coGetFileType
#define S_ISDIR(m) (((m) & S_IFMT) == S_IFDIR)  // coGetFileType

struct dirent {
	unsigned long     d_ino;	              /** Always zero. */
	unsigned short d_reclen;	      /** Always zero. */
	unsigned short d_namlen;            /** Length of name in d_name. */
	char		d_name[FILENAME_MAX]; /** File name. */
};

FILE *fp_target = NULL;
FILE *fp_non_target = NULL;
const char new_line[] = "\r\n";
int depth = 1;

//int printf_log(char *fmt, ...)
//{
//	va_list args;
//	char data[SIZE_BUFFER] = { 0, };
//
//	sprintf_s(data, SIZE_BUFFER, "LOG : ");
//	va_start(args, fmt);
//	vsprintf_s(data + strlen(data), SIZE_BUFFER - strlen(data), fmt, args);
//	va_end(args);
//
//	return printf("%s%s", data, new_line);
//}

int scandir(const char *dirname, struct dirent ***namelist,
	int(*select)(struct dirent *),
	int(*compar)(struct dirent **, struct dirent **))
{
	//char *d;
	WIN32_FIND_DATA find;
	HANDLE h;
	int nDir = 0, NDir = 0;
	struct dirent **dir = 0, *selectDir;
	unsigned long ret;
	char findfile[MAX_PATH];

	/*
	utf8tomb(dirname, strlen(dirname), findIn, _MAX_PATH);
	d = findIn + strlen(findIn);
	if (d == findIn) *d++ = '.';
	if (*(d - 1) != '/' && *(d - 1) != '\\') *d++ = '/';
	*d++ = '*';
	*d++ = 0;
	*/

	memset(findfile, 0x00, sizeof(findfile));
	sprintf_s(findfile, sizeof(findfile), "%s\\*", dirname);
	if ((h = FindFirstFile(findfile, &find)) == INVALID_HANDLE_VALUE)
	{
		ret = GetLastError();
		if (ret != ERROR_NO_MORE_FILES) {
			printf("Error: scandir() FindFirstFile %d\n", ret);
			return -1;
		}
		*namelist = dir;
		return nDir;
	}

	do
	{
		selectDir = (struct dirent*)malloc(sizeof(struct dirent));
		strcpy_s(selectDir->d_name, FILENAME_MAX, find.cFileName);
		if (!select || (*select)(selectDir)) {
			if (nDir == NDir) {
				struct dirent **tempDir = (struct dirent **)calloc(sizeof(struct dirent*), NDir + 33);
				if (NDir) memcpy(tempDir, dir, sizeof(struct dirent*)*NDir);
				if (dir) free(dir);
				dir = tempDir;
				NDir += 32;
			}
			dir[nDir] = selectDir;
			nDir++;
			dir[nDir] = 0;
		}
		else {
			free(selectDir);
		}
	} while (FindNextFile(h, &find));

	ret = GetLastError();
	if (ret != ERROR_NO_MORE_FILES) {
		printf("Error: scandir() FindNextFile %d\n", ret);
		return -1;
	}
	FindClose(h);

	if (compar) qsort(dir, nDir, sizeof(*dir),
		(int(*)(const void*, const void*))compar);

	*namelist = dir;
	return nDir;
}

int ScanDir(char *input_dir)
{
	struct dirent **c_entry_list;
	int c_count;
	int i;
	int ret_val = 0;
	struct stat statinfo;
	struct dirent *ent;
	char new_full_path[SIZE_BUFFER];
	char input_dir_full_path[SIZE_BUFFER];
	/* add recursive max depth by jhlim */

	_fullpath(input_dir_full_path, input_dir, SIZE_BUFFER);
	c_count = 0;

	if ((c_count = scandir(input_dir_full_path, &c_entry_list, 0, 0)) < 0)
	{
		printf("Error:(ScanDir) scandir (%s), errno = %d\n", input_dir_full_path, errno);
		return -1;
	}

	//if (fp_target != NULL)
	//{
	//	//for (int i = 0; i < depth; i++)
	//	//	fprintf(fp_target, "%s", "  ");
	//	fprintf(fp_target, "%s [%d]%s", input_dir_full_path, c_count - 2, new_line);
	//}
	printf("Inform: %s [%d]\n", input_dir_full_path, c_count - 2);

	for (i = 0; i < c_count; i++)
	{
		ent = c_entry_list[i];
		new_full_path[0] = 0;
		sprintf_s(new_full_path, SIZE_BUFFER, "%s%s%s", input_dir_full_path, PATH_SEPARATOR, ent->d_name);
		if (stat(new_full_path, &statinfo) < 0)
			continue;

		/* skip self and parent directory */
		if (strcmp(ent->d_name, "..") == 0 || strcmp(ent->d_name, ".") == 0)
			continue;

		/* 디렉토리 일때 */
		if (S_ISDIR(statinfo.st_mode))
		{
			/* recursive call [depth++] and tmp 에 원래경로 저장 */
			depth++;

			/* 재귀호출 */
			ret_val = ScanDir(new_full_path);

			/* recursive end [depth--] and 원래경로를 복사 */
			depth--;

			if (ret_val  < 0)
			{
				printf("Error: (ScanDir) ScanDir %s\n", new_full_path);
				return ret_val;
			}
		}
		else /* if(!S_ISDIR(statinfo.st_mode)) */
		{
			struct tm st_time;
			time_t time_sec;

			memset(&st_time, 0x00, sizeof(st_time));
			time_sec = 0;

			st_time.tm_year = 2018 - 1900;
			st_time.tm_mon = 2 - 1;
			st_time.tm_mday = 8;
			st_time.tm_hour = 12;

			time_sec = mktime(&st_time);

			if (time_sec < statinfo.st_mtime
				|| time_sec < statinfo.st_ctime)
			{
				fprintf(fp_target, "%s%s", new_full_path, new_line);
				_unlink(new_full_path);
			}
			else
			{
				fprintf(fp_non_target, "%s%s", new_full_path, new_line);
			}
		}/* if(!S_ISDIR(statinfo.st_mode)) */
	} /* for( i = 0; i < count; i++ ) */

	  /* free c_entry_list */
	if (c_count)
	{
		for (i = 0; i < c_count; i++)
			free(c_entry_list[i]);

		if (c_entry_list)
			free(c_entry_list);
	}
	return ret_val;
}

int main(int argc, char **argv)
{
	//char root[SIZE_BUFFER] = ".";
	char root[SIZE_BUFFER] = "Y:\\";

	if (argc > 1)
		strcpy_s(root, SIZE_BUFFER, argv[1]);

	fopen_s(&fp_target, "target_files.txt", "w+b");
	fopen_s(&fp_non_target, "non_target_files.txt", "w+b");
	ScanDir(root);
	fclose(fp_non_target);
	fclose(fp_target);
	return 0;
}