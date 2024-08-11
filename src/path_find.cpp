#include <iostream>
#include <vector>
#include <algorithm>
#include <stack>
#include <map>
#include <fstream>
#include <string>

using namespace std;

string infile = "input.txt";
string outfile = "output.txt";
const int setvsize = 1000;

int STATION = 0;
int TAXI = 0;
int st = -1, en = -1;
int mC;

map <int, int> ch;
vector <pair <int, int>> taxipath[setvsize];
vector <pair <int, int>> node[setvsize];
vector <int> rres;
vector <int> cP;

void pre() {
//   ifstream cin(infile);
   cin >> STATION >> st >> en;
   for (int i = 0; i < STATION * STATION - STATION; i++) {
      int q, w, c;
      cin >> q >> w >> c;
      node[q].push_back({ w, c });
   }
   cin >> TAXI;
   for (int i = 0; i < TAXI; i++) {
      int n;
      cin >> n;
      while (n--) {
         int q, w;
         cin >> q >> w;
         taxipath[i].push_back({ q, w });
      }
   }

}

void f(int cur, int n, int c, int co) {
   if (c == n) {
      if (co < mC) {
         mC = co;
         rres = cP;
      }
      return;
   }
   for (int i = 0; i < node[cur].size(); i++) {
      int nxt = node[cur][i].first, cost = node[cur][i].second;
      if (ch[nxt] && cost) {
         ch[nxt] = 0;
         cP.push_back(nxt);
         f(nxt, n, c + 1, co + cost);
         cP.pop_back();
         ch[nxt] = 1;
      }
   }
}
//st = 4, en = 3;
vector <pair <int, vector <int>>> solve() {
   vector <pair <int, vector <int>>> res;
   for (int i = 0; i < TAXI; i++) {
      vector <int> t;
      mC = 1e9;
      ch.clear();
      rres.clear();
      int top = taxipath[i][0].first;
      int count = 0;
      ch[st] = ch[en] = 1;
      for (auto path : taxipath[i]) {
         int q = path.first;
         int w = path.second;
         ch[q] = ch[w] = 1;
      }
      ch[top] = 0;
      for (int j = 0; j < STATION; j++) count += ch[j];
      f(top, count, 0, 0);
      t.push_back(top);
      for (auto tt : rres) t.push_back(tt);
      res.push_back({ i, t });
   }
   sort(res.begin(), res.end(),
      [](const pair <int, vector <int>>& mm, const pair <int, vector <int>>& nn) {
         vector <int> n, m;
         n = nn.second;
         m = mm.second;
         int mc = 0, nc = 0;
         for (int i = 0; i < m.size() - 1; i++) for (int j = 0; j < node[m[i]].size(); j++) if (node[m[i]][j].first == m[i + 1]) mc += node[m[i]][j].second;
         for (int i = 0; i < n.size() - 1; i++) for (int j = 0; j < node[n[i]].size(); j++) if (node[n[i]][j].first == n[i + 1]) mc += node[n[i]][j].second;
         return mc < nc;
      });
   return res;
}

int main() {
   pre();
   int sum = 0;
   int k = 1;
   vector <pair <int, vector <int>>> res;
   res = solve();
   printf("%d\n", res[k].first);
   for (int i : res[k].second) printf("%d ", i);
//   for (int i = 0; i < res[k].second.size()-1; i++)
//      for (int j = 0; j < node[res[k].second[i]].size(); j++)
//         if (node[res[k].second[i]][j].first == res[k].second[i + 1])
//            sum += node[res[k].second[i]][j].second;
//   printf("%d\n", sum);
   return 0;
}