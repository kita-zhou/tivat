#pragma once

#include<iostream>
#include<string>
#include<sys/types.h>
#include<sys/socket.h>
#include<stdlib.h>
#include<netinet/in.h>
#include<errno.h>
#include<arpa/inet.h>
#include<time.h>
#include<sys/time.h>
#include<unistd.h>
#include<chrono>


typedef unsigned int SOCKET; 
typedef sockaddr_in SOCKADDR_IN;
typedef sockaddr SOCKADDR;

std::string GetTime();



class MySocket
{
public:
	MySocket();
	~MySocket();
	bool Bind(std::string ip, int port);//��ip�Ͷ˿�
	bool Connect(std::string ip, int port);//���ӷ�����

	void Accept( SOCKET* client, SOCKADDR_IN* addr);//��������ʹ�ã�����һ������,���socket�͵�ַ
	
	bool Send(char* msg, int lenth);//���ͳ���Ϊlenth����Ϣ��socket
	bool Recive( char* msg,  int* lenth);//�ӷ�����������Ϣ������Ϊlenth, buff����4096�ֽ�

	bool MassiveSend(char* msg, int lenth);//�������ͳ���Ϊlenth����Ϣ��lenth�ϳ�ʱʹ�ã�
	bool MassiveRecive( char* msg, int lenth);//�������ճ���Ϊlenth����Ϣ��lenth�ϳ�ʱʹ�ã�

	bool GetAddr(SOCKADDR_IN* addr);

	void Close();//�ر�socket

	bool IsOpen();

	void Initial(SOCKET* sock);

	static bool WIN_INIT();//winapi��ʼ��
	static bool WIN_CLEAN();//winapi�ͷ���Դ

private:
	SOCKET mySocket;
};
