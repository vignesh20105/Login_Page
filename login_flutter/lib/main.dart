import 'package:flutter/material.dart';

void main(){
  runApp(const MyApp());
}

class MyApp extends StatelessWidget{
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
  
    return MaterialApp(
      debugShowCheckedModeBanner: false,
      title: 'login Application',
      home: Scaffold(
        body: Center(
          child: Text("Welcome to Flutter"),),
        ),
      );
    
  }
}