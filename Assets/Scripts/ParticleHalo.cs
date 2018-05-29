using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleHalo : MonoBehaviour {
	private ParticleSystem particleSys;  // 粒子系统  
	private ParticleSystem.Particle[] particleArr;  // 粒子
	private CirclePosition[] circle; // 极坐标

	public int count = 10000;       // 粒子数量  
	public float size = 0.03f;      // 粒子大小  
	public float minRadius = 5.0f;  // 最小半径  
	public float maxRadius = 12.0f; // 最大半径  
	public bool clockwise = true;   // 顺时针/逆时针  
	//public bool trigger = false;   // 是否点击中间加号的效果
	public float speed = 0.5f;        // 速度  
	public float pingPong = 0.02f;  // 游离范围 
	private float midRadius;  

	public Gradient colorGradient;  
	void Start ()  
	{   // 初始化粒子数组  
	    particleArr = new ParticleSystem.Particle[count];  
	    circle = new CirclePosition[count];  
	  
	    // 初始化粒子系统  
	    particleSys = this.GetComponent<ParticleSystem>();  
	    particleSys.startSpeed = 0;            // 粒子位置由程序控制  
	    particleSys.startSize = size;          // 粒子大小  
	    particleSys.loop = false;  
	    particleSys.maxParticles = count;      // 设置最大粒子量  
	    particleSys.Emit(count);               // 发射粒子  
	    particleSys.GetParticles(particleArr);  
	  
	    // 初始化梯度颜色控制器  
	    GradientAlphaKey[] alphaColors = new GradientAlphaKey[5];  
	    alphaColors[0].time = 0.0f; alphaColors[0].alpha = 1.0f;  
	    alphaColors[1].time = 0.4f; alphaColors[1].alpha = 0.4f;  
	    alphaColors[2].time = 0.6f; alphaColors[2].alpha = 1.0f;  
	    alphaColors[3].time = 0.9f; alphaColors[3].alpha = 0.4f;  
	    alphaColors[4].time = 1.0f; alphaColors[4].alpha = 0.9f;  
	    GradientColorKey[] colorKeys = new GradientColorKey[2];  
	    colorKeys[0].time = 0.0f; colorKeys[0].color = Color.white;  
	    colorKeys[1].time = 1.0f; colorKeys[1].color = Color.white;  
	    colorGradient.SetKeys(colorKeys, alphaColors);  
	  	midRadius = (maxRadius + minRadius) / 2;
	    RandomlySpread();   // 初始化各粒子位置  
	}  
	void RandomlySpread()  
	{  
	    for (int i = 0; i < count; ++i)  
	    {   // 随机每个粒子距离中心的半径，同时希望粒子集中在平均半径附近  
	        float minRate = Random.Range(1.0f, midRadius / minRadius);  
	        float maxRate = Random.Range(midRadius / maxRadius, 1.0f);  
	        float radius = Random.Range(minRadius * minRate, maxRadius * maxRate);  
	  
	        // 随机每个粒子的角度  
	        float angle = Random.Range(0.0f, 360.0f);  
	        float theta = angle / 180 * Mathf.PI;  
	  
	        // 随机每个粒子的游离起始时间  
	        float time = Random.Range(0.0f, 360.0f);  
	  
	        circle[i] = new CirclePosition(radius, angle, time);  
	  
	        particleArr[i].position = new Vector3(circle[i].radius * Mathf.Cos(theta), 0f, circle[i].radius * Mathf.Sin(theta));  
	    }  
	  
	    particleSys.SetParticles(particleArr, particleArr.Length);  
	}  
	// Update is called once per frame
	private int tier = 5;  // 速度差分层数  
	private int timeCount = 1;

	void Update ()  
	{  	

		for (int i = 0; i < count; i++)  {
			circle[i].angle = (360.0f + circle[i].angle) % 360.0f;  
			if(trigger) {
				if(clockwise) {//0~10内圈缩小, 10~20内圈放大，且光环半径缩小，光环宽度也缩小
					if(timeCount > 20 && timeCount < 30) { // 向内快速收缩
	        			circle[i].radius -= Mathf.PingPong(circle[i].time / minRadius / maxRadius, pingPong)*1f*(circle[i].radius + minRadius);  
	        		}
	        		else if(timeCount > 20 && timeCount < 40) { // 向外快速扩
	        			// 为了保障宽度缩小，缩小时半径越大，缩小越快，扩大时半径越大，扩大越慢
	        			circle[i].radius += Mathf.PingPong(circle[i].time / minRadius / maxRadius, pingPong)*3f *(maxRadius - circle[i].radius);
	        		}
	        		circle[i].angle -= (Random.Range(0,5) + 1) * (speed / circle[i].radius / tier); 
				} 
				else {	
					if(timeCount > 20 &&timeCount < 40) { // 向内快速收缩，光环半径缩小，光环宽度也缩小
	        			circle[i].radius -= Mathf.PingPong(circle[i].time / minRadius / maxRadius, pingPong)*3f*(circle[i].radius-minRadius) ;
	        		}
	        		circle[i].angle += (Random.Range(0,5) + 1) * (speed / circle[i].radius / tier); 
			        
				}
				
				
			}
			else {
				if (clockwise)  // 顺时针旋转  
			        circle[i].angle -= 0.1f;  
			    else            // 逆时针旋转  
			        circle[i].angle += 0.1f;  
		        // 保证angle在0~360度  
				circle[i].radius += Mathf.PingPong(circle[i].time / minRadius / maxRadius, pingPong) - pingPong / 2.0f;  
		         
			}

			float theta = circle[i].angle / 180 * Mathf.PI;  
			circle[i].time += Time.deltaTime;  
			particleArr[i].position = new Vector3(circle[i].radius * Mathf.Cos(theta), 0f, circle[i].radius * Mathf.Sin(theta));
			particleArr[i].color = colorGradient.Evaluate(circle[i].angle / 360.0f);    
		}
			
		particleSys.SetParticles(particleArr, particleArr.Length);  
	  	timeCount++;
	    
	}  
}
