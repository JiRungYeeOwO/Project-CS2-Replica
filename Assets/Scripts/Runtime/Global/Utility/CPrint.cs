using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// global using
//#error version

#region 유틸리티 : 프린트
/*
▶ 유틸리티 : 프린트


ㆍ 빌드 팁

- 릴리즈 빌드에서는 로그를 빼는게 정석
 ㄴ 개발자가 설계 / 유니티 명령어

- 명령어
 ㄴ [System.Diagnostics.Conditional("UNITY_EDITOR")]
 ㄴ [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
*/
#endregion

public static class CPrint
{
    public static bool Enable = true;
    public static bool EnableRichText = true;

    // 들여쓰기 레벨
    // 현재 들여쓰기 단계 (깊이) → 1이면 공백 2칸 붙음
    private static int _indentLevel = 0;
    // 1 단계 당 공백 몇 칸을 줄건지
    private const int INDENT_SPACES = 2;

    private static string Indent
    {
        // 레벨 2 → 공백 4칸 (2 * 2)
        get { return new string(' ', _indentLevel * INDENT_SPACES); }
    }

    // 들여쓰기 단계 올리기
    public static void IndentPush()
    {
        _indentLevel++;
    }

    // 들여쓰기 단계 내리기
    public static void IndentPop()
    {
        _indentLevel--;
        if (_indentLevel < 0) _indentLevel = 0;
    }

    /*
    CPrint.Title("Start");
    CPrint.IndentPush();
    CPrint.Log("아무 내용 A");
    CPrint.Log("아무 내용 B");
    CPrint.IndentPop();

    [Start]
      아무 내용 A
      아무 내용 B
    CPrint.IndentPop();
    → 다시 정상 출력
    */

    private enum ELogKind
    {
        Log,
        Warn,
        Error,
        Success
    }

    private static void Emit(ELogKind kind, string msg, string tag = null, string colorHex = null)
    {
        // 색상 값에 헥스를 쓰는 이유?
        // ㄴ 1. 문자열로 색을 표현하기 때문에 가장 범용적이다. (표준)
        // ㄴ 2. 16진수라서 깔끔하게 압축
        //  ㄴ R, G, B 각각 0 ~ 255값 → 16진수 2자리씩으로 표현 가능 (FF = 255 / 00 = 0)

        if (!Enable) return;

        string prefix = string.Empty;

        if (!string.IsNullOrEmpty(tag))
        {
            // IsNullOrEmpty : C# 함수 → 문자열이 쓸 수 있는 값인지 간단하게 검사한다.
            //  ㄴ EX : String.IsNullOrEmpty(s) → s == null / s == "" → true
            if (EnableRichText && !string.IsNullOrEmpty(colorHex))
            {
                prefix = $"<color={colorHex}>[{tag}]</color>";
            }
            else
            {
                prefix = $"[{tag}]";
            }
        }

        string final = $"{Indent}{prefix}{msg}";


        switch (kind)
        {
            case ELogKind.Log:
            case ELogKind.Success:
                Debug.Log(final);
                break;
            case ELogKind.Warn:
                Debug.LogWarning(final);
                break;
            case ELogKind.Error:
                Debug.LogError(final);
                break;
        }
    }


    // Title / Section
    public static void Title(string title, char lineCh = '=')
    {
        Line(lineCh);
        Emit(ELogKind.Log, title);
        Line(lineCh);
    }
    public static void Section(string section, char lineCh = '-')
    {
        Emit(ELogKind.Log, section);
        Line(lineCh);
    }

    // Line / Blank
    public static void Line(char ch = '=', int count = 10)
    {
        Emit(ELogKind.Log, new string(ch, count));
    }
    public static void Blank(int lines = 1)
    {
        // 예외
        if (!Enable) return;

        if (lines <= 0) return;
        Debug.Log(new string('\n', lines));
    }

    // Log / Warn / Error
    public static void Log(string msg)
    {
        Emit(ELogKind.Log, msg);
    }
    public static void Warn(string msg)
    {
        Emit(ELogKind.Warn, msg, "WARN", "#FF9100");

        // 필요 시 구글 → 헥스 색상표 검색
    }
    public static void Error(string msg)
    {
        Emit(ELogKind.Error, msg, "ERROR", "#FF1744");
    }

    public static void Success(string msg)
    {
        Emit(ELogKind.Success, msg, "OK", "#00C853");
    }

    // Assert
    public static void Assert(bool condition, string msg)
    {
        if (condition) return;

        Error($"[ASSERT] {msg}");
    }

    public static void CheckNull(object obj, string msg)
    {
        if (obj != null) return;

        Warn($"[NULL] {msg}");
    }

    // 참조 체크
    public static T Ref<T>(T obj, string msg) where T : class
    {
        if (obj == null)
        {
            Warn($"[NULL] {msg}");
        }

        // _rb = GetComponent<RigidBody)();
        // if (_rb == null) CPrint.Warn("....");

        // _rb = CPrint.Ref(GetComponent<RigidBody>(), "......");

        /*
        ▶ 제네릭 (가볍게)

        - 제네릭 → 타입을 나중에 정하는 설계
         ㄴ 함수 / 클래스를 만들 때 타입을 고정하지 않고 호출할 때 타입이 결정된다.

        EX :
        GetComponent<RigidBody>();  →   O
        GetComponent<Transform>();  →   O

        GetComponent<T>()

        - 우리가 만든 Ref 함수는 T 자리에 들어가는 타입이 달라져도 재사용 가능
         ㄴ 중복 코드를 뺀다.

        - T : 타입(타입 변수) 자리
         ㄴ int / Rigidbody / Transform 등등 타입이 들어올 자리

        - 제네릭은 <T>와 같은 제네릭 타입을 명시함으로서 정의하는 것이 가능


        ㆍ where T : class 작성 이유?

        - 제네릭은 기본적으로 모든 데이터 타입에 동작하도록 설계해야 한다.

        - 제네릭 클래스 또는 함수에 어떤 데이터 타입이 지정되어도 내부 로직에 변화가 발생하면 안된다.

        - 특정 데이터 타입에 동작하도록 데이터 타입을 제한하는 것이 가능하다.

        - T 클래스(참조형)만 받겠다는 제한


        ㆍ 타입 제한

        class CSomeClass<T> where T : class
         ㄴ 타입을 참조 형식으로 제한

        class CSomeClass<T> where T : struct
         ㄴ 타입을 값 형식으로 제한

        class CSomeClass<T> where T : SomeClass
         ㄴ 타입을 SomeClass를 직/간접적으로 상속하는 형식으로 제한

        class CSomeClass<T> where T : SomeInterface
         ㄴ 타입을 SomeInterface 직/간접적으로 따르는 형식으로 제한

        class CSomeClass<T> where T : U
         ㄴ 타입을 U(클래스 / 인터페이스)를 직/간접적으로 상속(따르는) 형식으로 제한
        */

        return obj;
    }

    // 좌표 가독성 (V3)
    public static void V3(string label, Vector3 v, int digits = 2)
    {
        float x = (float)System.Math.Round(v.x, digits);
        float y = (float)System.Math.Round(v.y, digits);
        float z = (float)System.Math.Round(v.z, digits);

        // Math.Round : 기본 수학 함수
        //  ㄴ 숫자를 가장 가까운 정수 또는 지정된 소수점 자릿수로 반올림

        Log($"{label} : ({x}, {y}, {z})");
    }

    public static void KV(string key, object value)
    {
        // Debug.Log($"HP = {hp});
        // CPrint.KV("HP", hp)

        Log($"{key} = {value}");
    }

    // Group : 하나의 로그 덩어리(섹션)을 만들어 주는 함수
    //  ㄴ 타이틀 찍고 → 들여쓰기 → 내용 → 들여쓰기 복구 → 라인
    public static void Group(string title, Action body, char lineCh = '=', int lineCount = 20)
    {
        if (!Enable) return;

        /*
        ▶ 델리게이트 (Delegate)
        
        - 델리게이트는 함수를 변수처럼 다룰 수 있게 해주는 타입
         ㄴ 특정 함수를 대신 호출해 주는 대리자

        - 프로그래밍에서 델리게이트는 콜백 함수를 의미한다.
         ㄴ 델리게이트를 이용하면 특정 이벤트가 발생하는 시점에 해당 이벤트를 처리하는 것이 가능하다.

        - 대리자는 자기가 가르키고 있는 함수를 호출하는 역할을 한다. → 함수에 대한 참조를 가지고 있어야 한다.

        [핵심]
        1. 실행을 위임한다.
        2. 호출 주체와 실행 주체가 같지 않다.

        
        ㆍ Action

        - Action은 델리게이트의 미리 만들어진 형태 (표준)
         ㄴ 3총사 : Action<T> Delegate / Func<T, TResult> Delegate / Predicate<T> Delegate

        - Action은 이 중에서도 매개 변수 없고 반환 값 없는 형태를 기본으로 제고하는 타입 (C#)
         ㄴ 실행 할 코드 덩어리를 변수처럼 전달 할 수 있다.
         ㄴ Group은 여기 안에 실행할 로그 / 코드 묶음을 통채로 받아서 실행하는 구조


        EX :
        CPrint.Group("프리셋 적용", () =>
        {
            CPrint.Log("1 : 총기 교체");
            CPrint.Log("2 : 렌더러 교체");
        });
        */
        //delegate Variable

        Title(title);
        IndentPush();
        body?.Invoke();
        IndentPop();
        Line();
    }

    // HashSet : 자료구조 → 중복을 허용하지 않고 고유한 요소만 저장하는 친구
    //  ㄴ 같은 값을 여러 번 Add해도 한번만 저장이 된다. / Contain()가 속도면에서 아주 좋다. (찾는 속도)

    /*
    ▶ HashSet

    - 컬렉션 클래스 중에 하나 → 해시 테이블 기반으로 구현된 집합 데이터 구조를 가지고 있다.
     ㄴ 탐색이 빠르고 추가 및 삭제 가능

    ㆍ 해시 테이블

    - 키 / 값으로 이뤄진 쌍 → 데이터를 저장하는 자료구조


    ㆍ 내부 동작

    01. 해시 함수
     ㄴ 값을 해시 코드로 바꾼다. (정수)
      ㄴ 같은 값이면 같은 해시 코드가 나오는게 목표
    
    02. 버킷
     ㄴ 해시 코드를 기준으로 저장 위치(버킷)를 고른다.
      ㄴ 대략적으로 해시코드 % 버킷갯수 같은 방식으로 인덱스를 결정한다고 이해하면 좋다.

    03. 충돌
     ㄴ 서로 다른 값인데 해시 코드가 겹칠 수 있다. (+ 버킷 위치)
     ㄴ 이 경우 버킷 안에서 추가 비교로 진짜 같은 값인지 확인하는 절차를 수행
      ㄴ HashSet → 해시 + 실제 비교를 같이 쓴다.

    04. 재해싱
     ㄴ 요소가 너무 많아져서 버킷이 타이트해 지면 성능이 떨어진다.
      ㄴ 더 큰 테이블을 만들고 다시 배치한다. (재해싱)
       ㄴ 비용이 들기 때문에 → 주의해야 한다.


    ㆍ readonly

    const vs readonly

    readonly : 참조형일때는 참조 자체는 고정 → 그 객체 내부 값 변경은 허용하는 경우가 있다.
    */
    private static readonly HashSet<string> _onceSet = new HashSet<string>();

    public static void Once(string key, string msg)
    {
        if (!Enable) return;

        // key가 이미 있으면 이미 경고한 내용이니까 재출력 금지
        if (_onceSet.Contains(key)) return;

        _onceSet.Add(key);

        Warn($"[ONCE] {msg}");

        // EX :
        //  ㄴ CPrint.Once("Player", "Player가 비어 있다. 인스펙터에서 확인");
    }

    public static void OnceClear()
    {
        _onceSet.Clear();
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
    public static void Ray(Vector3 origin, Vector3 direction, Color color, float duration = 0f)
    {
        if (!Enable) return;

        Debug.DrawRay(origin, direction, color, duration);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
    public static void Line3D(Vector3 a, Vector3 b, Color color, float duration = 0f)
    {
        if (!Enable) return;

        Debug.DrawLine(a, b, color, duration);
    }

}
